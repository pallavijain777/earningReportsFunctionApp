using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Models
{
    public class Report
    {
        public long ReportId { get; set; }
        public int CompanyId { get; set; }
        public int QuarterId { get; set; }
        public string BlobName { get; set; } = null!;
        public string? BlobUrl { get; set; }
        public string? BlobVersion { get; set; }
        public string Source { get; set; } = "manual";
        public string Status { get; set; } = "Pending";
        public DateTime UploadedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public Company Company { get; set; } = null!;
        public FiscalQuarter Quarter { get; set; } = null!;
        public ReportInsight? Insight { get; set; }
        public ICollection<ProcessingRun> Runs { get; set; } = new List<ProcessingRun>();
    }
}
