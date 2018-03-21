using System;

namespace Subscriber.Entities
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Stadium { get; set; }
        public string Country { get; set; }
    }
}