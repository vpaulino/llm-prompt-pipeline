using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Enrichers.DataProviders;

namespace AIConversationalAgent.Enrichers
{
    public class UserEnricher : IPromptEnricher
    {
        private readonly IUserDataProvider _userRepository;

        public UserEnricher(IUserDataProvider userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task EnrichAsync(PromptContext promptContext)
        {
            var scopes = promptContext.Get<List<string>>("ExtractedScopes");
            if (scopes == null || !scopes.Any()) return;

            var users = await _userRepository.GetUsersByTopicsAsync(scopes);
            if (users.Any())
            {
                string userList = string.Join("\n", users.Select(u => $"- {u.Name} ({u.Email})"));
                promptContext.Request.System += $"\n\n### Relevant Users:\n{userList}";
                promptContext.Set("EnrichedUsers", users);
         
            }
        }
    }

}
