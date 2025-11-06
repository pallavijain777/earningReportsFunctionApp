using Azure.Storage.Blobs;
using EarningsReportsFunctionApp.Helper;
using EarningsReportsFunctionApp.Plugins;
using EarningsReportsFunctionApp.Repositories.Implementations;
using EarningsReportsFunctionApp.Repositories.Interfaces;
using EarningsReportsFunctionApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

[assembly: FunctionsStartup(typeof(EarningsReportsFunctionApp.Startup))]

namespace EarningsReportsFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            string keyVaultUrl = config["KEY_VAULT_URL"] ?? throw new InvalidOperationException("Key Vault not found");

            // Initialize KeyVault service
            var keyVault = new KeyVaultService(keyVaultUrl);

            // Fetch secrets securely
            string azureOpenAiKey = keyVault.GetSecret("AZURE-OPENAI-KEY")
                ?? throw new InvalidOperationException("Missing secret: AZURE-OPENAI-KEY in Key Vault.");

            string azureOpenAiEndpoint = keyVault.GetSecret("AZURE-OPENAI-ENDPOINT")
                ?? throw new InvalidOperationException("Missing secret: AZURE-OPENAI-ENDPOINT in Key Vault.");

            string azureOpenAiDeployment = keyVault.GetSecret("AZURE-OPENAI-DEPLOYMENT")
                ?? throw new InvalidOperationException("Missing secret: AZURE-OPENAI-DEPLOYMENT in Key Vault.");

            string sqlConnection = keyVault.GetSecret("SQL-CONNECTION-STRING")
                ?? throw new InvalidOperationException("Missing secret: SQL-CONNECTION-STRING in Key Vault.");

            string blobConnection = keyVault.GetSecret("AZURE-STORAGE-CONNECTION")
                ?? throw new InvalidOperationException("Missing secret: AZURE-STORAGE-CONNECTION in Key Vault.");


            // Register Semantic Kernel + Azure OpenAI
            builder.Services.AddSingleton<Kernel>(sp =>
            {
                var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(azureOpenAiDeployment, azureOpenAiEndpoint, azureOpenAiKey)
                    .Build();

                // Import your plugin into the kernel automatically
                kernel.ImportPluginFromObject(new FinancialAnalysisPlugin(kernel), "FinancialAnalysis");

                return kernel;
            });

            // BlobServiceClient
            builder.Services.AddSingleton(x =>
                new BlobServiceClient(blobConnection));

            // SQL + EF Core
            builder.Services.AddDbContext<InvestelloReportsDbContext>(options => options.UseSqlServer(sqlConnection));

            // Repositories + Services
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IFiscalQuarterRepository, FiscalQuarterRepository>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IReportInsightRepository, ReportInsightRepository>();
            builder.Services.AddScoped<IProcessingRunRepository, ProcessingRunRepository>();
            builder.Services.AddScoped<IReportProcessingService, ReportProcessingService>();
            builder.Services.AddSingleton<SemanticKernelService>();
        }
    }
}
