using System.Collections.Generic;

namespace BlazorApp.Shared.Models
{
    public class Subscription
    {
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string DisplayName { get; set; }
        public SubscriptionState State { get; set; }
        public IReadOnlyDictionary<string, string> Tags { get; set; }
    
        public Subscription(string subscriptionId, string subscriptionName, string displayName, SubscriptionState state, IReadOnlyDictionary<string, string> tags)
        {
            SubscriptionId = subscriptionId;
            SubscriptionName = subscriptionName;
            DisplayName = displayName;
            State = state;
            Tags = tags;
        }
    }
    
}

