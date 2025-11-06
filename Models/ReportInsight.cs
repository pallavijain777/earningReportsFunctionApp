using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Models
{
    public class ReportInsight
    {
        public long InsightId { get; set; }
        public long ReportId { get; set; }
        public string? Summary { get; set; }
        public int SentimentScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public Report Report { get; set; } = null!;
    }
}
