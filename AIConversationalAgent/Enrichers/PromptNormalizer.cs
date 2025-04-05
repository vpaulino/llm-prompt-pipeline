using AIConversationalAgent.ApiModels;
using System.Text.Json;

namespace AIConversationalAgent.Enrichers
{
    public class PromptNormalizer : IPromptEnricher
    {
        private readonly IModelEngineService _modelEngine;

        public PromptNormalizer(IModelEngineService modelEngine)
        {
            _modelEngine = modelEngine;
        }

        public async Task EnrichAsync(PromptContext context)
        {
            var normalizationRequest = new ConversationRequest
            {
                Model = context.Request.Model,
                System = @"You are an assistant that extracts structured details from user requests. 
                       Analyze the user prompt and return a JSON object with the fields:
                       - 'What': The main action requested.
                       - 'How': The method of execution, should be email, sms, whatsapp or pushNotifications.
                       - 'Who': The target audience. What characterizes the users that I need to select
                       - 'When': The relevant time context. It should be a date
                       - 'KeywordsContext': List of keywords useful for filtering.",
                Prompt = context.Request.Prompt,
                Format = "json",
                Stream = false,
                Context = promptContext.Context
            };

            var response = await _modelEngine.GenerateAsync(normalizationRequest, CancellationToken.None);
            var extractedSchema = ParseSchema(response.Response);
            promptContext.Request.System += "the structured details are " + response;
            // Store extracted details in the context
            context.Set("NormalizedSchema", extractedSchema);
        }

        private NormalizedSchema ParseSchema(string jsonResponse)
        {
            try
            {
                return JsonSerializer.Deserialize<NormalizedSchema>(jsonResponse) ?? new NormalizedSchema();
            }
            catch
            {
                return new NormalizedSchema();
            }
        }
    }

    public class NormalizedSchema
    {
        public string What { get; set; } = string.Empty;
        public string How { get; set; } = string.Empty;
        public string Who { get; set; } = string.Empty;
        public string When { get; set; } = string.Empty;
        public List<string> KeywordsContext { get; set; } = new();
    }

}
