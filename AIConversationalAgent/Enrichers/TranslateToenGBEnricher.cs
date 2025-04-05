
using AIConversationalAgent.ApiModels;

namespace AIConversationalAgent.Enrichers
{
    public class TranslateToenGBEnricher : IPromptEnricher
    {
        private readonly IModelEngineService _modelEngine;
        

        public TranslateToenGBEnricher(IModelEngineService modelEngine)
        {
            _modelEngine = modelEngine;
        }

        public async Task EnrichAsync(PromptContext promptContext)
        {
            var eventExtractionRequest = new ConversationRequest
            {
                Model = promptContext.Request.Model,
                Prompt = promptContext.Request.Prompt,
                System = $"Based on the follow up text that Im sending to you translate this text into english en-GB. This text is a Bill Statement and contains information about the company emmiting this bill, the person to pay and values to pay and tax that should be kept in euros.",
                Format = "json",
                Stream = false,
                Context = promptContext.Context
            };

            var eventResponse = await _modelEngine.GenerateAsync(eventExtractionRequest, CancellationToken.None);

            promptContext.Set("TranslateToenGBEnricher", eventResponse.Response);
        }
    }
}
