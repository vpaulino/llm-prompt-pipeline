using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Enrichers.DataProviders;
using System.Text.Json;

namespace AIConversationalAgent.Enrichers
{
    public class EventMetadataEnricher : IPromptEnricher
    {
        private readonly IModelEngineService _modelEngine;
        private readonly IEventDataProvider _eventRepository;

        public EventMetadataEnricher(IModelEngineService modelEngine, IEventDataProvider eventRepository)
        {
            _modelEngine = modelEngine;
            _eventRepository = eventRepository;
        }

        public async Task EnrichAsync(PromptContext promptContext)
        {
            // 1️⃣ Extract Event Name from LLM
            var eventExtractionRequest = new ConversationRequest
            {
                Model = promptContext.Request.Model,
                Prompt = $"Extract also  the event name to the json field name 'event_name'. Return only the event name as a plain string.",
                Format = "json",
                Stream = false,
                Context = promptContext.Context
            };

            var eventResponse = await _modelEngine.GenerateAsync(eventExtractionRequest, CancellationToken.None);
            var extractedEventName = ExtractEventName(eventResponse.Response);

            if (string.IsNullOrEmpty(extractedEventName))
            {
                Console.WriteLine("⚠️ No event name extracted from prompt.");
                return;
            }

            // 2️⃣ Query Database for Event Details
            var eventDetails = await _eventRepository.GetEventByNameAsync(extractedEventName);
            if (eventDetails == null)
            {
                Console.WriteLine($"⚠️ No event found in the database for name: {extractedEventName}");
                return;
            }

            promptContext.Set("EventDetails", eventDetails);
            promptContext.Context = eventResponse.Context;
            // 3️⃣ Enrich the System Prompt with Event Metadata
            promptContext.Request.System += $"\n\n### Event Details:\n" +
                              $"- Name: {eventDetails.Name}\n" +
                              $"- Location: {eventDetails.Location}\n" +
                              $"- Date: {eventDetails.Date:MMMM dd, yyyy}\n" +
                              $"- Description: {eventDetails.Description}";
        }

        private string ExtractEventName(string rawResponse)
        {
            try
            {
                var json = JsonSerializer.Deserialize<Dictionary<string, string>>(rawResponse);
                return json.ContainsKey("event_name") ? json["event_name"] : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

}
