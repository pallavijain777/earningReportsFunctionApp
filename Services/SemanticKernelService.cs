using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Threading.Tasks;
// Don't forget to include the namespace for your new Plugin
using EarningsReportsFunctionApp.Plugins;

namespace EarningsReportsFunctionApp.Services
{
    public class SemanticKernelService
    {
        private readonly Kernel _kernel;

        public SemanticKernelService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<(string summary, int sentimentscore)> AnalyzeReportAsync(string reportText)
        {
            // Prepare arguments to pass the report text to the functions
            var arguments = new KernelArguments
            {
                ["reportText"] = reportText
            };

            var summaryResult = await _kernel.InvokeAsync<string>("FinancialAnalysis", "SummarizeReport", arguments);

            var sentimentResult = await _kernel.InvokeAsync<int>("FinancialAnalysis", "AnalyzeSentiment", arguments);

            return (summaryResult, sentimentResult);
        }
    }
}