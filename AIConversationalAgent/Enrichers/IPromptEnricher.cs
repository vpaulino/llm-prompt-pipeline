using AIConversationalAgent.ApiModels;

namespace AIConversationalAgent.Enrichers
{
    public interface IPromptEnricher
    {
        Task EnrichAsync(PromptContext context);
    }


}
