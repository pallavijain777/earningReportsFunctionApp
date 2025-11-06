using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Interfaces
{
    public interface IReportInsightRepository
    {
        Task<ReportInsight?> GetByReportIdAsync(long reportId, CancellationToken ct = default);
        Task<ReportInsight> UpsertAsync(ReportInsight insight, CancellationToken ct = default);
    }
}
