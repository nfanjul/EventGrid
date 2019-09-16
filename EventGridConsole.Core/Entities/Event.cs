using System;

namespace EventGridConsole.Core.Entities
{
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid().ToString();
            EventType = "NFC.EventGrid.TeamData";
            EventTime = DateTime.UtcNow.ToString("o");
        }

        public Team UpdateProperties
        {
            set
            {
                Subject = $"{value.Name}/{value.Stadium}/{value.Country}"; ;
                Data = value;
            }
        }

        public string Id { get; }

        public string Subject { get; set; }

        public string EventType { get; }

        public string EventTime { get; }

        public Team Data { get; set; }

    }
}
