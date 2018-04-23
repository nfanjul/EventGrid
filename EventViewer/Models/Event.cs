namespace EventViewer.Models
{
    public class Event<TEntity>
    {
        public string Id { get; set; }

        public string Subject { get; set; }

        public string EventType { get; set; }

        public string EventTime { get; set; }

        public TEntity Data { get; set; }

        public string Topic { get; set; }
    }
}
