using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarningsReportsFunctionApp.Helper
{
    public class KeyVaultService
    {
        private readonly SecretClient _client;

        public KeyVaultService(string keyVaultUrl)
        {
            _client = new SecretClient(new Uri(keyVaultUrl), new AzureCliCredential());
        }

        public string? GetSecret(string name)
        {
            try
            {
                var response = _client.GetSecret(name);
                return response.Value.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Key Vault error: {ex.Message}");
                return null;
            }
        }
    }
}
