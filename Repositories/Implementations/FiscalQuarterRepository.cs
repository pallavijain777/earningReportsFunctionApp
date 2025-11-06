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
    public class FiscalQuarterRepository : IFiscalQuarterRepository
    {
        private readonly InvestelloReportsDbContext _db;
        public FiscalQuarterRepository(InvestelloReportsDbContext db) { _db = db; }
        public async Task<FiscalQuarter> GetByYearQuarterAsync(int fiscalYear, byte quarterNum, CancellationToken ct = default)
            => await _db.FiscalQuarters.FirstOrDefaultAsync(x => x.FiscalYear == fiscalYear && x.QuarterNum == quarterNum, ct) ?? null!;
       
    }
}
