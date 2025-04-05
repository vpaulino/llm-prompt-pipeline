using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Enrichers;

public interface IModelEngineService
{
        Task<ConversationResponse> GenerateAsync(ConversationRequest request, CancellationToken token);
        IAsyncEnumerable<string> StreamAsync(ConversationRequest request, CancellationToken token);
        Task<IEnumerable<string>> GetAvailableModelsAsync(CancellationToken token);
        Task EnrichAsync(ConversationRequest request, IEnumerable<IPromptEnricher> enrichers, CancellationToken token);
}
