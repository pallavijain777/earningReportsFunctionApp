using EarningsReportsFunctionApp.Models;
using EarningsReportsFunctionApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Services
{
    public class ReportProcessingService : IReportProcessingService
    {
        private readonly ICompanyRepository _companies;
        private readonly IFiscalQuarterRepository _quarters;
        private readonly IReportRepository _reports;
        private readonly IReportInsightRepository _insights;
        private readonly IProcessingRunRepository _runs;
        private readonly InvestelloReportsDbContext _db;

        public ReportProcessingService(ICompanyRepository companies, IFiscalQuarterRepository quarters, IReportRepository reports, IReportInsightRepository insights, IProcessingRunRepository runs, InvestelloReportsDbContext db)
        {
            _companies = companies;
            _quarters = quarters;
            _reports = reports;
            _insights = insights;
            _runs = runs;
            _db = db;
        }

        public async Task<(int CompanyId, int QuarterId, long ReportId)> EnsureReportAsync(string symbol, string name, int fiscalYear, byte quarterNum, string blobName, string? blobUrl, CancellationToken ct = default)
        {
            var company = await _db.Companies.FirstOrDefaultAsync(x => x.Symbol == symbol, ct);
            if (company == null)
            {
                company = new Company { Symbol = symbol, Name = name, CreatedAt = DateTime.UtcNow };
                _db.Companies.Add(company);
                await _db.SaveChangesAsync(ct);
            }

            var quarter = await _db.FiscalQuarters.FirstOrDefaultAsync(x => x.FiscalYear == fiscalYear && x.QuarterNum == quarterNum, ct);
            if (quarter == null)
            {
                quarter = new FiscalQuarter { FiscalYear = fiscalYear, QuarterNum = quarterNum };
                _db.FiscalQuarters.Add(quarter);
                await _db.SaveChangesAsync(ct);
            }

            var report = await _reports.GetByCompanyQuarterAsync(company.CompanyId, quarter.QuarterId, ct);
            if (report == null)
            {
                report = new Report { CompanyId = company.CompanyId, QuarterId = quarter.QuarterId, BlobName = blobName, BlobUrl = blobUrl, UploadedAt = DateTime.UtcNow, Status = "Pending" };
                await _reports.AddAsync(report, ct);
            }

            return (company.CompanyId, quarter.QuarterId, report.ReportId);
        }

        public async Task MarkReportStatusAsync(long reportId, string status, CancellationToken ct = default)
        {
            var report = await _db.Reports.FirstAsync(x => x.ReportId == reportId, ct);
            report.Status = status;
            if (status == "Processed") report.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task SaveInsightsAsync(long reportId, string summary, int sentiment, CancellationToken ct = default)
        {
            var insight = new ReportInsight { ReportId = reportId, Summary = summary, SentimentScore = sentiment,CreatedAt = DateTime.UtcNow};
            await _insights.UpsertAsync(insight, ct);
        }

        public Task<long> StartRunAsync(long reportId, Guid? invocationId, CancellationToken ct = default)
            => StartRun(reportId, invocationId, ct);

        private async Task<long> StartRun(long reportId, Guid? invocationId, CancellationToken ct)
        {
            var run = await _runs.StartAsync(reportId, invocationId, ct);
            return run.RunId;
        }

        public Task CompleteRunAsync(long runId, bool success, int? tokensIn, int? tokensOut, decimal? costUsd, string? error, CancellationToken ct = default)
            => _runs.CompleteAsync(runId, success ? "Succeeded" : "Failed", tokensIn, tokensOut, costUsd, error, ct);
    }
}
