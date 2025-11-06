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
    public class ProcessingRunRepository : IProcessingRunRepository
    {
        private readonly InvestelloReportsDbContext _db;
        public ProcessingRunRepository(InvestelloReportsDbContext db) { _db = db; }
        public async Task<ProcessingRun> StartAsync(long reportId, Guid? invocationId, CancellationToken ct = default)
        {
            var run = new ProcessingRun { ReportId = reportId, FunctionInvocationId = invocationId, StartedAt = DateTime.UtcNow, Status = "Started" };
            _db.ProcessingRuns.Add(run);
            await _db.SaveChangesAsync(ct);
            return run;
        }
        public async Task CompleteAsync(long runId, string status, int? tokensIn, int? tokensOut, decimal? costUsd, string? error, CancellationToken ct = default)
        {
            var run = await _db.ProcessingRuns.FirstAsync(x => x.RunId == runId, ct);
            run.Status = status;
            run.TokensInput = tokensIn;
            run.TokensOutput = tokensOut;
            run.CostUSD = costUsd;
            run.ErrorMessage = error;
            run.EndedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
