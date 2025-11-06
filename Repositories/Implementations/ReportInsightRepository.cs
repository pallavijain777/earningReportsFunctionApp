using EarningsReportsFunctionApp.Models;
using EarningsReportsFunctionApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Implementations
{
    public class ReportInsightRepository : IReportInsightRepository
    {
        private readonly InvestelloReportsDbContext _db;
        public ReportInsightRepository(InvestelloReportsDbContext db) { _db = db; }
        public async Task<ReportInsight?> GetByReportIdAsync(long reportId, CancellationToken ct = default)
            => await _db.ReportInsights.FirstOrDefaultAsync(x => x.ReportId == reportId, ct);
        public async Task<ReportInsight> UpsertAsync(ReportInsight insight, CancellationToken ct = default)
        {
            var existing = await _db.ReportInsights.FirstOrDefaultAsync(x => x.ReportId == insight.ReportId, ct);
            if (existing == null) _db.ReportInsights.Add(insight);
            else
            {
                existing.Summary = insight.Summary;
                existing.SentimentScore = insight.SentimentScore;
            }
            await _db.SaveChangesAsync(ct);
            return existing ?? insight;
        }
    }
}
