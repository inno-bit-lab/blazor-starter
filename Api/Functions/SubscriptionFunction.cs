using Azure.Identity;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using BlazorApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using Azure;

namespace IblFunction.Functions
{
    public class SubscriptionFunction
    {
        private readonly ILogger<SubscriptionFunction> log;

        public SubscriptionFunction(ILogger<SubscriptionFunction> logger)
        {
            log = logger;
        }

        [Function("Subscription")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            //return this.returnDummy();
            try
            {
                log.LogInformation("SubscriptionFunction: GetSubscriptions HTTP trigger function processed a request.");
                // Usa DefaultAzureCredential per autenticare
                var credential = new DefaultAzureCredential();

                log.LogInformation("SubscriptionFunction: Creating ArmClient");
                // Crea il client Azure Resource Manager
                var armClient = new ArmClient(credential);
             
                log.LogInformation("SubscriptionFunction: Getting subscriptions");
                // Ottieni le sottoscrizioni
                var subscriptions = armClient.GetSubscriptions();
                log.LogInformation("SubscriptionFunction: Subscriptions retrieved");
                var subscriptionList = new List<Subscription>();
                foreach (SubscriptionResource subscription in subscriptions)
                {
                    log.LogInformation($"SubscriptionFunction: Processing subscription {subscription.Data.SubscriptionId}");
                    // Estrai i dati necessari dal SubscriptionResource
                    string subscriptionId = subscription.Data.SubscriptionId;
                    string subscriptionName = subscription.Data.SubscriptionId; // Nota: Azure non fornisce un subscription name, puoi usare l'ID
                    string displayName = subscription.Data.DisplayName;

                    // Mappare l'enum di Azure a quello personalizzato
                    SubscriptionState state = MapSubscriptionState(subscription.Data.State);

                    IReadOnlyDictionary<string, string> tags = subscription.Data.Tags;

                    log.LogInformation($"SubscriptionFunction: Creating Subscription object for {subscriptionId}");
                    // Crea l'oggetto Subscription
                    var subscriptionObj = new Subscription(subscriptionId, subscriptionName, displayName, state, tags);

                    // Aggiungi l'oggetto alla lista
                    subscriptionList.Add(subscriptionObj);
                    log.LogInformation($"SubscriptionFunction: Subscription {subscriptionId} processed");
                }

                log.LogInformation("SubscriptionFunction: Returning subscriptions");
                // Ritorna la lista di sottoscrizioni come JSON
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteAsJsonAsync(subscriptionList);
                return response;
            }
            catch (Exception ex)
            {
                log.LogError($"Errore durante l'ottenimento delle sottoscrizioni: {ex.Message}");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteAsJsonAsync("Exception in request: " + ex.Message);
                return response;
            }
        }

        private HttpResponseData returnDummy(HttpRequestData req)
        {

            log.LogInformation("Willt return dummy info");
            var subscriptionList = new List<Subscription>();
            subscriptionList.Add(new Subscription("1", "Subscription 1", "Display Name 1", SubscriptionState.Enabled, new Dictionary<string, string>()));
            subscriptionList.Add(new Subscription("2", "Subscription 2", "Display Name 2", SubscriptionState.Enabled, new Dictionary<string, string>()));
            subscriptionList.Add(new Subscription("3", "Subscription 3", "Display Name 3", SubscriptionState.Enabled, new Dictionary<string, string>()));
            subscriptionList.Add(new Subscription("4", "Subscription 4", "Display Name 4", SubscriptionState.Enabled, new Dictionary<string, string>()));
            // Ritorna la lista di sottoscrizioni come JSON
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(subscriptionList);
            return response;
        }

        private SubscriptionState MapSubscriptionState(Azure.ResourceManager.Resources.Models.SubscriptionState? azureState)
        {
            return azureState switch
            {
                Azure.ResourceManager.Resources.Models.SubscriptionState.Enabled => SubscriptionState.Enabled,
                Azure.ResourceManager.Resources.Models.SubscriptionState.Warned => SubscriptionState.Warned,
                Azure.ResourceManager.Resources.Models.SubscriptionState.PastDue => SubscriptionState.PastDue,
                Azure.ResourceManager.Resources.Models.SubscriptionState.Disabled => SubscriptionState.Disabled,
                Azure.ResourceManager.Resources.Models.SubscriptionState.Deleted => SubscriptionState.Deleted,
                _ => SubscriptionState.Unknow
            };
        }
    }
}
