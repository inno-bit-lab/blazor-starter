using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlazorApp.Shared.Models;

namespace BlazorApp.Client.Services
{
    public class AzureSubscriptionService(HttpClient httpClient)
    {
        public async Task<List<Subscription>?> GetAzureSubscriptionsAsync()
        {
            try
            {
                Console.WriteLine("Without Token Call to https://iblfunction.azurewebsites.net/api/GetSubscriptions: ");
                var request = new HttpRequestMessage(HttpMethod.Get,
                    "https://iblfunction.azurewebsites.net/GetSubscriptions");
                Console.WriteLine("Requesting Azure Subscriptions");
                Subscription[] fromJsonAsync = await httpClient.GetFromJsonAsync<Subscription[]>("/api/Subscription") ?? new Subscription[]{};
                
                return fromJsonAsync.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }

            return [];
        }
    }
}