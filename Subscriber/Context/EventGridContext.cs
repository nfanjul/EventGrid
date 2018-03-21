using Subscriber.Entities;
using System.Configuration;
using System.Data.Entity;

namespace Subscriber.Context
{
    public class EventGridContext: DbContext
    {
        public EventGridContext() : base(ConfigurationManager.ConnectionStrings["EventGridContext"].ConnectionString)
        {
        }

        public DbSet<Team> Teams { get; set; }

    }
}