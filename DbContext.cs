using EarningsReportsFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EarningsReportsFunctionApp
{
    public class InvestelloReportsDbContext : DbContext
    {
        public InvestelloReportsDbContext(DbContextOptions<InvestelloReportsDbContext> options) : base(options) { }
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<FiscalQuarter> FiscalQuarters => Set<FiscalQuarter>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportInsight> ReportInsights => Set<ReportInsight>();
        public DbSet<ProcessingRun> ProcessingRuns => Set<ProcessingRun>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Company>(e =>
            {
                e.ToTable("Company");
                e.HasKey(x => x.CompanyId);
                e.HasIndex(x => x.Symbol).IsUnique();
                e.Property(x => x.Symbol).IsRequired().HasMaxLength(20);
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.Exchange).HasMaxLength(50);
                e.Property(x => x.ISIN).HasMaxLength(20);
                e.Property(x => x.Sector).HasMaxLength(100);
            });

            b.Entity<FiscalQuarter>(e =>
            {
                e.ToTable("FiscalQuarter");
                e.HasKey(x => x.QuarterId);
                e.HasIndex(x => new { x.FiscalYear, x.QuarterNum }).IsUnique();
                e.Property(x => x.QuarterNum).HasColumnType("tinyint");
                e.Ignore(x => x.Label);
            });

            b.Entity<Report>(e =>
            {
                e.ToTable("Report");
                e.HasKey(x => x.ReportId);
                e.HasIndex(x => new { x.CompanyId, x.QuarterId }).IsUnique();
                e.Property(x => x.BlobName).IsRequired().HasMaxLength(300);
                e.Property(x => x.BlobUrl).HasMaxLength(500);
                e.Property(x => x.Source).HasMaxLength(30);
                e.Property(x => x.Status).HasMaxLength(20);
                e.HasOne(x => x.Company).WithMany(x => x.Reports).HasForeignKey(x => x.CompanyId);
                e.HasOne(x => x.Quarter).WithMany(x => x.Reports).HasForeignKey(x => x.QuarterId);
                e.HasOne(x => x.Insight).WithOne(x => x.Report).HasForeignKey<ReportInsight>(x => x.ReportId);
            });

            b.Entity<ReportInsight>(e =>
            {
                e.ToTable("ReportInsight");
                e.HasKey(x => x.InsightId);
                e.Property(x => x.SentimentScore).HasColumnType("tinyint");
            });

            b.Entity<ProcessingRun>(e =>
            {
                e.ToTable("ProcessingRun");
                e.HasKey(x => x.RunId);
                e.Property(x => x.Status).HasMaxLength(20);
                e.HasOne(x => x.Report).WithMany(x => x.Runs).HasForeignKey(x => x.ReportId);
            });
        }
    }
}
