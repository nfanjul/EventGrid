using Newtonsoft.Json;
using Subscriber.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Subscriber
{
    public class WebApiApplication : HttpApplication
    {
        protected async void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            await CreateSubscriptions();
        }

        private async Task CreateSubscriptions()
        {
            var subscriptions = new List<Subscription>
            {
                new Subscription { Name = "All" },
                new Subscription { Name = "Team", PrefixFilter = "Team" },
            };
            foreach (var subscription in subscriptions)
            {
                if (await SubscriptionExists(subscription.Name))
                {
                    continue;
                }
                await CreateSubscription(subscription.Name, subscription.PrefixFilter, subscription.SuffixFilter);
            }
        }

        private static async Task<bool> SubscriptionExists(string subscription)
        {
            var result = await CreateHttpClient()
                .GetAsync(
                    $"https://management.azure.com/subscriptions/" + $"{ConfigurationManager.AppSettings["SubscriptionID"]}/resourceGroups/{ConfigurationManager.AppSettings["ResourceGroup"]}/providers/Microsoft.EventGrid/topics/{ConfigurationManager.AppSettings["EventGridTopic"]}/providers/Microsoft.EventGrid/eventSubscriptions/{subscription}?api-version=2018-01-01");
            return result.IsSuccessStatusCode;
        }

        private static async Task CreateSubscription(string subscription, string prefixFilter, string suffixFilter)
        {
            var createSubscription = new
            {
                properties = new
                {
                    destination = new { endpointType = "webhook", properties = new { endpointUrl = $"{ConfigurationManager.AppSettings["ApiAppUrl"]}/api/Subscribers/{subscription}" } },
                    filter = new { includedEventTypes = new[] { "teamData" }, subjectBeginsWith = prefixFilter, subjectEndsWith = suffixFilter, subjectIsCaseSensitive = "false" }
                }
            };
            var json = JsonConvert.SerializeObject(createSubscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await CreateHttpClient()
                .PutAsync(
                    $"https://management.azure.com/subscriptions/" + $"{ConfigurationManager.AppSettings["SubscriptionID"]}/resourceGroups/{ConfigurationManager.AppSettings["ResourceGroup"]}/providers/Microsoft.EventGrid/topics/{ConfigurationManager.AppSettings["EventGridTopic"]}/providers/Microsoft.EventGrid/eventSubscriptions/{subscription}?api-version=2018-01-01",
                    content);
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            return httpClient;
        }

    }
}
