using CustomTopic.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTopic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<Settings>(Configuration.GetSection("Connections"));
            CreateSubscriptions().Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private async Task CreateSubscriptions()
        {
            var subscriptions = new List<Subscription>
            {

                new Subscription { Name = "All" },
                new Subscription { Name = "Hydra", PrefixFilter = "Hydra" },
                new Subscription { Name = "Repairs", SuffixFilter = "Repair" },
                new Subscription { Name = "AerisOrders", PrefixFilter = "Aeris", SuffixFilter = "Order" }
            };
            foreach (var subscription in subscriptions)
            {
                if (await SubscriptionExists(subscription.Name))
                {
                    continue;
                }
                await CreateSubscription(subscription.Name, subscription.PrefixFilter, subscription.SuffixFilter);
                Thread.Sleep(5000);
            }
        }

        private static async Task<bool> SubscriptionExists(string subscription)
        {
            var result = await CreateHttpClient()
                .GetAsync(
                    $"https://management.azure.com/subscriptions/{CloudConfigurationManager.GetSetting("SubscriptionID")}/resourceGroups/{CloudConfigurationManager.GetSetting("ResourceGroup")}/providers/Microsoft.EventGrid/topics/{CloudConfigurationManager.GetSetting("EventGridTopic")}/providers/Microsoft.EventGrid/eventSubscriptions/{subscription}?api-version=2017-06-15-preview");
            return result.IsSuccessStatusCode;
        }

        private static async Task CreateSubscription(string subscription, string prefixFilter, string suffixFilter)
        {
            var createSubscription = new
            {
                properties = new
                {
                    destination = new { endpointType = "webhook", properties = new { endpointUrl = $"{CloudConfigurationManager.GetSetting("ApiAppUrl")}/api/Subscribers/{subscription}" } },
                    filter = new { includedEventTypes = new[] { "shipevent" }, subjectBeginsWith = prefixFilter, subjectEndsWith = suffixFilter, subjectIsCaseSensitive = "false" }
                }
            };
            var json = JsonConvert.SerializeObject(createSubscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await CreateHttpClient()
                .PutAsync(
                    $"https://management.azure.com/subscriptions/{CloudConfigurationManager.GetSetting("SubscriptionID")}/resourceGroups/{CloudConfigurationManager.GetSetting("ResourceGroup")}/providers/Microsoft.EventGrid/topics/{CloudConfigurationManager.GetSetting("EventGridTopic")}/providers/Microsoft.EventGrid/eventSubscriptions/{subscription}?api-version=2017-06-15-preview",
                    content);
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {CloudConfigurationManager.GetSetting("AuthorizationToken")}");
            return httpClient;
        }

    }
}
