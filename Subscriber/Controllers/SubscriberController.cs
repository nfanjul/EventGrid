using Subscriber.Context;
using Subscriber.Entities;
using Subscriber.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Subscriber.Controllers
{
    public class SubscriberController : ApiController
    {

        [ActionName("Team")]
        public IHttpActionResult Post([FromBody] List<EventModel> value)
        {
            InsertToTable(value);        
            return Ok();
        }

        private void InsertToTable(List<EventModel> value)
        {
            if (value == null || value.Count == 0)
            {
                return;
            }
            var context = new EventGridContext();
            context.Teams.Add(new Team { Id = Guid.NewGuid(), Country = value[0].Data.Country, Name = value[0].Data.Name, Stadium = value[0].Data.Stadium });
            context.SaveChanges();
        }

    }
}
