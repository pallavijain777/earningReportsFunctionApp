using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EarningsReportsFunctionApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Logging;


namespace EarningsReportsFunctionApp.Functions
{
    public class ProcessPendingReports
    {
        private readonly BlobServiceClient _blobServiceClient; 
        private readonly IReportProcessingService _reportProcessor;
        private readonly ILogger<ProcessPendingReports> _logger;
        private readonly SemanticKernelService _skService;

        public ProcessPendingReports(BlobServiceClient blobServiceClient, IReportProcessingService reportProcessor, SemanticKernelService skService, ILogger<ProcessPendingReports> logger)
        {
            _blobServiceClient = blobServiceClient;
            _reportProcessor = reportProcessor;
            _skService = skService;
            _logger = logger;
        }

        private static string ExtractPdfText(Stream pdfStream)
        {
            pdfStream.Position = 0;
            using var pdf = PdfDocument.Open(pdfStream);
            var sb = new StringBuilder();

            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString();
        }

        [FunctionName("ProcessPendingReports")]
        public async Task RunAsync([TimerTrigger("0 0 12 * * *")] TimerInfo timer)
        {
            var invocationId = Guid.NewGuid();
            _logger.LogInformation($"Triggered at {DateTime.UtcNow} | InvocationId: {invocationId}");

            var container = _blobServiceClient.GetBlobContainerClient("earnings-reports");

            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                long reportId = 0;
                long runId = 0;
                string companyCode = string.Empty;

                try
                {
                    var blobClient = container.GetBlobClient(blob.Name);

                    // Fetch existing metadata
                    var properties = await blobClient.GetPropertiesAsync();
                    var metadata = new Dictionary<string, string>(properties.Value.Metadata, StringComparer.OrdinalIgnoreCase);

                    // Skip if already processed
                    if (metadata.TryGetValue("processed", out string? processed) && processed.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation($"Skipping {blob.Name} — already processed.");
                        continue;
                    }

                    using var ms = new MemoryStream();
                    await blobClient.DownloadToAsync(ms);
                    ms.Position = 0;

                    // Extract company + fiscal info from path
                    var parts = blob.Name.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 3)
                    {
                        _logger.LogWarning($"Skipping blob '{blob.Name}' – unexpected path structure.");
                        continue;
                    }

                    companyCode = parts[0];              // e.g. "tcs"
                    string fiscalInfo = parts[1];        // e.g. "2025_Q1"
                    string fileName = parts[2];          // e.g. "report.pdf"
                    string blobUrl = blobClient.Uri.ToString();

                    // Parse fiscal year and quarter
                    var fiscalParts = fiscalInfo.Split('_');
                    if (fiscalParts.Length != 2 || !int.TryParse(fiscalParts[0], out int fiscalYear))
                    {
                        _logger.LogWarning($"Skipping blob '{blob.Name}' – invalid fiscal info format.");
                        continue;
                    }

                    byte quarterNum = byte.Parse(fiscalParts[1].Substring(1)); 

                    _logger.LogInformation($"📄 Processing {companyCode} | FY {fiscalYear} Q{quarterNum} | File: {fileName}");

                    // 1️ Ensure company/report exist
                    var (companyId, quarterId, ensuredReportId) = await _reportProcessor.EnsureReportAsync(
                        symbol: companyCode.ToUpper(),
                        name: companyCode.ToUpper(),
                        fiscalYear: fiscalYear,
                        quarterNum: quarterNum,
                        blobName: blob.Name,
                        blobUrl: blobUrl
                    );
                    reportId = ensuredReportId;

                    // 2️ Start processing run
                    runId = await _reportProcessor.StartRunAsync(
                        reportId,
                        invocationId: invocationId
                    );

                    // 3️ Extract PDF text
                    string text = ExtractPdfText(ms);
                    if (string.IsNullOrWhiteSpace(text))
                        throw new Exception("Extracted PDF text is empty.");

                    // 4️ Run AI analysis
                    var (summary, score) = await _skService.AnalyzeReportAsync(text);

                    _logger.LogInformation($"Summary generated.");
                    _logger.LogInformation($"Sentiment Score: {score}");

                    // 5️ Save results to DB
                    await _reportProcessor.SaveInsightsAsync(
                        reportId: reportId,
                        summary: summary,
                        sentiment: (byte)score
                    );

                    await _reportProcessor.MarkReportStatusAsync(reportId, "Processed");

                    // 6️ Update blob metadata — mark as processed
                    metadata["processed"] = "true";
                    metadata["processedAt"] = DateTime.UtcNow.ToString("o");
                    metadata["reportId"] = reportId.ToString();
                    await blobClient.SetMetadataAsync(metadata);

                    // 7 Complete run (success)
                    await _reportProcessor.CompleteRunAsync(
                        runId,
                        success: true,
                        tokensIn: null,     
                        tokensOut: null,
                        costUsd: null,
                        error: null
                    );

                    _logger.LogInformation($"Completed processing for {companyCode} | ReportId={reportId} | RunId={runId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing report for {companyCode}. ReportId={reportId} RunId={runId}");

                    if (reportId > 0)
                        await _reportProcessor.MarkReportStatusAsync(reportId, "Failed");

                    if (runId > 0)
                    {
                        await _reportProcessor.CompleteRunAsync(
                            runId,
                            success: false,
                            tokensIn: null,
                            tokensOut: null,
                            costUsd: null,
                            error: ex.Message
                        );
                    }
                }
            }

            _logger.LogInformation($"Cycle completed at {DateTime.UtcNow}");
        }
    }
}
