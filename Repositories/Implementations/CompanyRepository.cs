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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly InvestelloReportsDbContext _db;
        public CompanyRepository(InvestelloReportsDbContext db) { _db = db; }
        public async Task<Company> GetBySymbolAsync(string symbol, CancellationToken ct = default)
            => await _db.Companies.FirstOrDefaultAsync(x => x.Symbol == symbol, ct) ?? null!;

        public async Task<Company> AddAsync(Company company, CancellationToken ct = default)
        {
            _db.Companies.Add(company);
            await _db.SaveChangesAsync(ct);
            return company;
        }

    }
}
