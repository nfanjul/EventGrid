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
        private const string TOPIC_ENDPOINT = "https://mytopicnfc.francecentral-1.eventgrid.azure.net/api/events";
        private const string KEY = "fWVgrHfctmNcw+IeAY5JASngt4oxo0N1Da3OK7bycWA=";

        static void Main(string[] args)
        {
            while (true)
            {
                #region Console code
                Console.Title = "Net Core Conf - 28 de septiermbre de 2019";

                Console.WriteLine("Net Core Conf - 28 de septiermbre de 2019" + "\n" + "\n");
                Console.WriteLine("                                .oMc");
                Console.WriteLine("                             .MMMMMP");
                Console.WriteLine("                           .MM888MM");
                Console.WriteLine("     ....                .MM88888MP");
                Console.WriteLine("     MMMMMMMMb.         d8MM8tt8MM");
                Console.WriteLine("      MM88888MMMMc `:' dMME8ttt8MM");
                Console.WriteLine("       MM88tt888EMMc:dMM8E88tt88MP");
                Console.WriteLine("        MM8ttt888EEM8MMEEE8E888MC");
                Console.WriteLine("        `MM888t8EEEM8MMEEE8t8888Mb");
                Console.WriteLine("         \"MM88888tEM8\"MME88ttt88MM");
                Console.WriteLine("          dM88ttt8EM8\"MMM888ttt8MM");
                Console.WriteLine("          MM8ttt88MMº\" \" \"MMNICKMM\"");
                Console.WriteLine("          3M88888MM\"      \"MMMP\"");
                Console.WriteLine("           \"MNICKM\"");
                Console.WriteLine("\n" + "\n");

                Console.Write("Introduzca la información del equipo:" + "\n");
                Console.WriteLine("Nombre: ");
                var name = Console.ReadLine();
                Console.WriteLine("Estadio: ");
                var stadium = Console.ReadLine();
                Console.WriteLine("Pais: ");
                var country = Console.ReadLine();
                #endregion
                // Show 6
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
            var httpClient = new HttpClient();

            // SHOW 7
            httpClient.DefaultRequestHeaders.Add("aeg-sas-key", KEY);
            var json = JsonConvert.SerializeObject(events);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("Sending event to Event Grid...");
            // SHOW 8 FIN
            var result = await httpClient.PostAsync(TOPIC_ENDPOINT, content);

  
            Console.WriteLine($"Event sent with result: {result.ReasonPhrase}");
            Console.WriteLine();
        }

    }
}
