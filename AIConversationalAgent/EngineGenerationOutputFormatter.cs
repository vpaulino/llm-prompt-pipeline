using AIConversationalAgent.Enrichers.Models;

namespace AIConversationalAgent
{
    public class EngineGenerationOutputFormatter
    {
        public async Task<string> FormatAsync(Template template, ConversationResponse? response)
        {
            await Task.CompletedTask;
            return response.Response;
        }
    }
}