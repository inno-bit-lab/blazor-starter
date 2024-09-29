using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorApp.Shared.Models;

namespace BlazorApp.Client.Services
{
    public class AzureSubscriptionService(HttpClient httpClient)
    {
        public async Task<List<Subscription>?> GetAzureSubscriptionsAsync()
        {
            try
            {
                Console.WriteLine("Without Token Call to /api/Subscription: ");
                Console.WriteLine("Requesting Azure Subscription");
                string value = await httpClient.GetStringAsync("/api/Subscription");
                Console.WriteLine($"Function return jsonstring: {value}");
                Console.WriteLine("calling mapping with Subscription object");
                var functionResponse =
                    await httpClient.GetFromJsonAsync<SubscriptionFunctionResponse>("/api/Subscription");
                    //??
                    //new SubscriptionFunctionResponse(httpStatusCode: HttpStatusCode.NoContent, new FunctionResponseError("999", "No content response from function"));
                if(functionResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Mapping to Subscription object");
                    return functionResponse.Content;
                }
                else if (functionResponse.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("No content response from function");
                    throw new Exception("No content response from function");
                }else 
                {
                    var content = $"Code: {functionResponse.Error.Code}, Message: {functionResponse.Error.Message}";
                    Console.WriteLine("Failed to get Azure Subscriptions response: " + functionResponse.HttpStatusCode + " return msg string: " + content);
                    throw new Exception("Failed to get Azure Subscriptions response: " + functionResponse.HttpStatusCode + " return msg string: " + content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unmanaged Exception: " + ex);
                throw new Exception("Unmanaged Exception: " + ex.Message);

            }
        }
    }
}