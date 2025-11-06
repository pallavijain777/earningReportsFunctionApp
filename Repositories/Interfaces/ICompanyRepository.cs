using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> GetBySymbolAsync(string symbol, CancellationToken ct = default);
        Task<Company> AddAsync(Company company, CancellationToken ct = default);
    }
}
