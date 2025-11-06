using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<Report?> GetByCompanyQuarterAsync(int companyId, int quarterId, CancellationToken ct = default);
        Task<Report> AddAsync(Report report, CancellationToken ct = default);
        Task<List<Report>> GetPendingAsync(int take, CancellationToken ct = default);
        Task UpdateAsync(Report report, CancellationToken ct = default);
    }
}
