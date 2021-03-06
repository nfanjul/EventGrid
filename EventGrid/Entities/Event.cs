﻿using System;

namespace EventGridConsole.Entities
{
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid().ToString();
            EventType = "teamData";
            EventTime = DateTime.UtcNow.ToString("o");
        }

        public Team AddProperties
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
