using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Common
{
    public class KeyVaultHelper
    {
        static KeyVaultClient keyVaultClient;
        static string KeyVaultUrl;

        static KeyVaultHelper()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        static async Task<string> GetSecret(string name)
        {

            try
            {
                var secret = await keyVaultClient.GetSecretAsync($"https://{KeyVaultUrl}.vault.azure.net/secrets/{name}").ConfigureAwait(false);
                return secret.Value; 
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        static public async Task<string> GetAccessTokenAsync()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");
            return accessToken;
        }

    }
}
