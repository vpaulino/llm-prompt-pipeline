namespace AIConversationalAgent.Enrichers.Models
{
    public class Event
    {
        public string Name { get; set; }
        public string Location { get; internal set; }
        public DateTime Date { get; internal set; }
        public string Description { get; internal set; }
    }
}