using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Repositories.Interfaces
{
   
    public interface IFiscalQuarterRepository
    {
        Task<FiscalQuarter> GetByYearQuarterAsync(int fiscalYear, byte quarterNum, CancellationToken ct = default);
    }

}
