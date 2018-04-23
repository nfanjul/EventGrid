using EventGridConsole.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventGridConsole
{
    class Program
    {
        private const string TOPIC_ENDPOINT = "[URL CUSTOM TOPIC}";

        private const string KEY = "[CUSTOM TOPIC KEY]";

        static void Main(string[] args)
        {
            while(true)
            {
                #region Console code
                Console.Title = "Global Azure Bootcamp - 21 de abril de 2018";         
                Console.Write("Introduzca la información del equipo:" + "\n");
                Console.WriteLine("Nombre: ");
                var name = Console.ReadLine();
                Console.WriteLine("Estadio: ");
                var stadium = Console.ReadLine();
                Console.WriteLine("Pais: ");
                var country = Console.ReadLine();
                #endregion
                var events = new List<Event>
                {
                    new Event { UpdateProperties = new Team { Name = name, Stadium = stadium, Country = country, Type = "All" } }
                };
                SendEventsToTopic(events).Wait();
                Console.WriteLine("-------------------------- FIN! --------------------------");
            }
        }

        private static async Task SendEventsToTopic(object events)
        {
            // Create a HTTP client which we will use to post to the Event Grid Topic
            var httpClient = new HttpClient();

            // Add key in the request headers
            httpClient.DefaultRequestHeaders.Add("aeg-sas-key", KEY);

            // Event grid expects event data as JSON
            var json = JsonConvert.SerializeObject(events);

            // Create request which will be sent to the topic
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send request
            Console.WriteLine("Sending event to Event Grid...");
            var result = await httpClient.PostAsync(TOPIC_ENDPOINT, content);

            // Show result
            Console.WriteLine($"Event sent with result: {result.ReasonPhrase}");
            Console.WriteLine();
        }

    }
}
