using Azure.Identity;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using BlazorApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
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
                await foreach (SubscriptionResource subscription in subscriptions)
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
                return new OkObjectResult(subscriptionList);
            }
            catch (Exception ex)
            {
                log.LogError($"Errore durante l'ottenimento delle sottoscrizioni: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<IActionResult> returnDummy()
        {
            log.LogInformation("Willt return dummy info");
            return null;
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
