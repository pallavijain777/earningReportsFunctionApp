using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Plugins
{
    [Description("Provides financial analysis capabilities like summarization and sentiment.")]
    public class FinancialAnalysisPlugin
    {
        private readonly Kernel _kernel;

        public FinancialAnalysisPlugin(Kernel kernel)
        {
            _kernel = kernel;
        }

        [KernelFunction("SummarizeReport")]
        [Description("Summarizes a detailed financial earnings report into 8-10 key sentences.")]
        public async Task<string> SummarizeReportAsync([Description("The full text of the earnings report to summarize.")] string reportText)
        {
            var summaryPrompt = $"""
                You are a financial analyst. Summarize the following earnings report in 10-12 sentences.    
                Keep the language concise and neutral.

                Report:
                {reportText}
                """;

            string? response = await _kernel.InvokePromptAsync<string>(summaryPrompt);

            // Handle null or empty responses safely
            if (string.IsNullOrWhiteSpace(response))
            {
                return "No summary could be generated due to an empty response.";
            }

            return response.Trim();
        }

        [KernelFunction("AnalyzeSentiment")]
        [Description("Analyzes the sentiment of an earnings report.")]
        public async Task<int> AnalyzeSentimentAsync([Description("The full text of the earnings report to analyze sentiment.")] string reportText)
        {
            // Define prompt template (the model only gets clear JSON instructions)
            var prompt = $"""
                You are a financial analyst. Analyze the sentiment of the given earnings report text.
                Respond only with a score between 1 and 10 with 10 being the most positive.

                Earnings Report:
                {reportText}
                """;

            string? response = await _kernel.InvokePromptAsync<string>(prompt);

            // Step 2: Try parsing directly
            if (int.TryParse(response?.Trim(), out int sentiment))
            {
                return sentiment;
            }

            // Step 3: Fallback if model included words (e.g., “Sentiment score: 8”)
            var match = Regex.Match(response ?? string.Empty, @"\d+");
            if (match.Success && int.TryParse(match.Value, out sentiment))
            {
                return sentiment;
            }

            // Step 4: Default fallback
            return 0;
        }
    }
}