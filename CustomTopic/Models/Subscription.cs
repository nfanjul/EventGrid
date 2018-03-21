namespace CustomTopic.Models
{
    public class Subscription
    {
        public string Name { get; set; }
        
        public string PrefixFilter { get; set; }

        public string SuffixFilter { get; set; }
    }
}
