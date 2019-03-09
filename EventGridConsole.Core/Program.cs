using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EventGridConsole.Core.Entities;
using Newtonsoft.Json;

namespace EventGridConsole.Core
{
    class Program
    {
        //private const string TOPIC_ENDPOINT = "https://eventgrid.azurewebsites.net/api/Subscriber";
        private const string TOPIC_ENDPOINT = "http://localhost:5000/api/Subscriber";

        private const string KEY = "/FA6v0naObQNKh539Ob6zjG7vyMNtk8Tggukx7xP/DY=";

        static void Main(string[] args)
        {
            while (true)
            {
                #region Console code
                Console.Title = "Global Integration Bootcamp - 30 de marzo de 2019";
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
