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
    public class ReportRepository : IReportRepository
    {
        private readonly InvestelloReportsDbContext _db;
        public ReportRepository(InvestelloReportsDbContext db) { _db = db; }
        public async Task<Report?> GetByCompanyQuarterAsync(int companyId, int quarterId, CancellationToken ct = default)
            => await _db.Reports.Include(x => x.Insight).FirstOrDefaultAsync(x => x.CompanyId == companyId && x.QuarterId == quarterId, ct);
        public async Task<Report> AddAsync(Report report, CancellationToken ct = default)
        {
            _db.Reports.Add(report);
            await _db.SaveChangesAsync(ct);
            return report;
        }
        public async Task<List<Report>> GetPendingAsync(int take, CancellationToken ct = default)
            => await _db.Reports.Where(x => x.Status == "Pending").OrderBy(x => x.UploadedAt).Take(take).ToListAsync(ct);
        public async Task UpdateAsync(Report report, CancellationToken ct = default)
        {
            _db.Reports.Update(report);
            await _db.SaveChangesAsync(ct);
        }
    }
}
