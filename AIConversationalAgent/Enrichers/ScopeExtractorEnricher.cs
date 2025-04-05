using AIConversationalAgent.ApiModels;
using System.Text.Json;

namespace AIConversationalAgent.Enrichers
{
    public class ScopeExtractorEnricher : IPromptEnricher
    {
        private readonly IModelEngineService _modelEngine;

        public ScopeExtractorEnricher(IModelEngineService modelEngine)
        {
            _modelEngine = modelEngine;
        }

        public async Task EnrichAsync(PromptContext promptContext)
        {
            try
            {

                var scopeExtractionRequest = new ConversationRequest
                {
                    Model = promptContext.Request.Model,
                    Prompt = $"Extract the  scopes from the prompt {promptContext.Request}. Return a comma-separated list in the json field 'scopes'.",
                    Format = "json",
                    Stream = false, 
                    Context = promptContext.Context,
                };

                var scopeResponse = await _modelEngine.GenerateAsync(scopeExtractionRequest, CancellationToken.None);
                var extractedScopes = ExtractScopesFromResponse(scopeResponse.Response);

                promptContext.Request.System += "the structured details are " + response;
                promptContext.Set("ExtractedScopes", extractedScopes);

                promptContext.Context = scopeResponse.Context;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private List<string> ExtractScopesFromResponse(string response)
        {
            try
            {
                var json = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
                return json.ContainsKey("scopes") ? json["scopes"].Split(',').Select(s => s.Trim()).ToList() : new();
            }
            catch
            {
                return new();
            }
        }
    }

}
