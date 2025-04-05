using AIConversationalAgent.Enrichers.Models;

namespace AIConversationalAgent.Enrichers.DataProviders
{
    public interface IUserDataProvider
    {
        Task<IEnumerable<User>> GetUsersByTopicsAsync(IEnumerable<string> topics);
    }

    public class InMemoryUserDataProvider : IUserDataProvider
    {
        private readonly List<User> _users;

        public InMemoryUserDataProvider()
        {
            // Seed with example users
            _users = new List<User>
        {
            new User { Id = 1, Name = "Alice", Email = "alice@example.com", Topics = new List<string> { "AI", "Web" } },
            new User { Id = 2, Name = "Bob", Email = "bob@example.com", Topics = new List<string> { "Database", "Security" } },
            new User { Id = 3, Name = "Charlie", Email = "charlie@example.com", Topics = new List<string> { "AI", "Security" } }
        };
        }

        public Task<IEnumerable<User>> GetUsersByTopicsAsync(IEnumerable<string> topics)
        {
            var matchingUsers = _users
                .Where(user => user.Topics.Any(topic => topics.Contains(topic, StringComparer.OrdinalIgnoreCase)))
                .ToList();

            return Task.FromResult<IEnumerable<User>>(matchingUsers);
        }
    }
     

}