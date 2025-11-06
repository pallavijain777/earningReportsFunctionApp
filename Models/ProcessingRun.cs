using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Models
{
    public class ProcessingRun
    {
        public long RunId { get; set; }
        public long ReportId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Status { get; set; } = null!;
        public Guid? FunctionInvocationId { get; set; }
        public int? TokensInput { get; set; }
        public int? TokensOutput { get; set; }
        public decimal? CostUSD { get; set; }
        public string? ErrorMessage { get; set; }
        public Report Report { get; set; } = null!;
    }
}
