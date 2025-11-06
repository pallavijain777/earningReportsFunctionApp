using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Models
{
    public class FiscalQuarter
    {
        public int QuarterId { get; set; }
        public int FiscalYear { get; set; }
        public byte QuarterNum { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string Label { get; private set; } = null!;
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
