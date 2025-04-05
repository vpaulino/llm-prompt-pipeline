using AIConversationalAgent.Enrichers.Models;

namespace AIConversationalAgent.Enrichers.DataProviders
{
    public interface IEventDataProvider
    {
        Task<Event> GetEventByNameAsync(string extractedEventName);
    }

    public class InMemoryEventDataProvider : IEventDataProvider
    {
        private readonly List<Event> _events;

        public InMemoryEventDataProvider()
        {
            // Seed with one example event
            _events = new List<Event>
            {
                new Event
                {
                    Name = "Tech of the Future",
                    Location = "New York, USA",
                    Date = new DateTime(2025, 4, 20),
                    Description = "A conference about AI and Web3."
                }
            };
        }

        public Task<Event?> GetEventByNameAsync(string extractedEventName)
        {
            var eventMatch = _events.FirstOrDefault(e => e.Name.Equals(extractedEventName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(eventMatch);
        }
    }
     

}