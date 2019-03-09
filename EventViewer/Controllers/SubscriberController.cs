using EventViewer.Hubs;
using EventViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EventViewer.Controllers
{
    [Produces("application/json")]
    [Route("api/Subscriber")]
    public class SubscriberController : Controller
    {
        private bool EventTypeSubcriptionValidation => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation";

        private bool EventTypeNotification => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification";

        private IHubContext<EventGridHub> _hubContext;

        public SubscriberController(IHubContext<EventGridHub> context)
        {
            _hubContext = context;
        }

        [HttpPost]
        public IActionResult Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var jsonContent = reader.ReadToEndAsync().Result;
                if (EventTypeSubcriptionValidation)
                {
                    var gridEvent = JsonConvert.DeserializeObject<List<Event<Dictionary<string, string>>>>(jsonContent).First();
                    var validationCode = gridEvent.Data["validationCode"];
                    return new JsonResult(new
                    {
                        validationResponse = validationCode
                    });
                }
                else if (EventTypeNotification)
                {
                    var events = JArray.Parse(jsonContent);
                    foreach (var item in events)
                    {
                        var details = JsonConvert.DeserializeObject<Event<dynamic>>(item.ToString());
                        _hubContext.Clients.All.SendAsync(
                            "eventgridupdate",
                            details.Id,
                            details.EventType,
                            details.Subject,
                            details.EventTime.ToString(),
                            item.ToString());
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            return Ok("Hello Post");
        }

    }
}