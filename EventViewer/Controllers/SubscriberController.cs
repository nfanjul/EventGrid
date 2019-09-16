using EventViewer.Hubs;
using EventViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventViewer.Controllers
{
    [Produces("application/json")]
    [Route("api/Subscriber")]
    public class SubscriberController : Controller
    {
        #region Data Members

        private readonly IHubContext<EventGridHub> _hubContext;

        #endregion

        #region Constructors

        public SubscriberController(IHubContext<EventGridHub> eventGridHubContext)
        {
            _hubContext = eventGridHubContext;
        }

        #endregion

        #region Public Methods

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string requestContent = await reader.ReadToEndAsync();
                EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();
                EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);
                foreach (EventGridEvent eventGridEvent in eventGridEvents)
                {
                    if (eventGridEvent.Data is SubscriptionValidationEventData)
                    {
                        var validationResult = await ValidationHandler(eventGridEvent);
                        return Ok(validationResult);
                    }
                    await EventCustomHandler(eventGridEvent);
                    return Ok();
                }
            }
            return Ok();
        }

        #endregion

        #region Private Methods

        private async Task<SubscriptionValidationResponse> ValidationHandler(EventGridEvent eventGridEvent)
        {
            await EventCustomHandler(eventGridEvent);

            var eventData = (SubscriptionValidationEventData)eventGridEvent.Data;
            return new SubscriptionValidationResponse()
            {
                ValidationResponse = eventData.ValidationCode
            };
        }

        private async Task EventCustomHandler(EventGridEvent eventGridEvent)
        {
            await _hubContext.Clients.All.SendAsync(
                "gridupdate",
                eventGridEvent.Id,
                eventGridEvent.EventType,
                eventGridEvent.Subject,
                eventGridEvent.EventTime.ToString(),
                JsonConvert.SerializeObject(eventGridEvent.Data));
        }

        #endregion
    }
}