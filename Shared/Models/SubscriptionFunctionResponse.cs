using System.Collections.Generic;
using System.Net;

namespace BlazorApp.Shared.Models
{
    public class SubscriptionFunctionResponse : FunctionResponse<List<Subscription>>
    {
        public SubscriptionFunctionResponse() : base() { }


        public SubscriptionFunctionResponse(HttpStatusCode httpStatusCode, List<Subscription> content) : base(httpStatusCode, content)
        {
        }

        public SubscriptionFunctionResponse(HttpStatusCode httpStatusCode, FunctionResponseError error) : base(httpStatusCode, error)
        {
        }
    }
}