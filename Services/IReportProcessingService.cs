using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Services
{
    public interface IReportProcessingService
    {
        Task<(int CompanyId, int QuarterId, long ReportId)> EnsureReportAsync(string symbol, string name, int fiscalYear, byte quarterNum, string blobName, string blobUrl, CancellationToken ct = default);
        Task MarkReportStatusAsync(long reportId, string status, CancellationToken ct = default);
        Task SaveInsightsAsync(long reportId, string summary, int sentiment, CancellationToken ct = default);
        Task<long> StartRunAsync(long reportId, Guid? invocationId, CancellationToken ct = default);
        Task CompleteRunAsync(long runId, bool success, int? tokensIn, int? tokensOut, decimal? costUsd, string error, CancellationToken ct = default);
    }
}
