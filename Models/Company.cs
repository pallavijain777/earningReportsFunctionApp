using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Symbol { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Exchange { get; set; }
        public string? ISIN { get; set; }
        public string? Sector { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
