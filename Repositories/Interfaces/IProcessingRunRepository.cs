using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Interfaces
{
    public interface IProcessingRunRepository
    {
        Task<ProcessingRun> StartAsync(long reportId, Guid? invocationId, CancellationToken ct = default);
        Task CompleteAsync(long runId, string status, int? tokensIn, int? tokensOut, decimal? costUsd, string? error, CancellationToken ct = default);
    }
}
