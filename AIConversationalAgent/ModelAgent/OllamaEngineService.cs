using AIConversationalAgent;
using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Configuration;
using AIConversationalAgent.Enrichers;
using System.Text.Json;

public class OllamaEngineService : IModelEngineService
{
    private readonly HttpClient _httpClient;

    public OllamaEngineService(HttpClient httpClient, IConfiguration config)
    {
        var modelSettings = config.GetSection("llm").GetSection("Models").Get<List<ModelConfig>>() ?? new List<ModelConfig>();
        var ollamaConfig = modelSettings.FirstOrDefault(m => m.Name.Equals("ollama", StringComparison.OrdinalIgnoreCase));

        if (ollamaConfig == null)
        {
            throw new InvalidOperationException("Ollama configuration not found in appsettings.json");
        }

        _httpClient = new HttpClient { BaseAddress = new Uri(ollamaConfig.Url) };
    }

    public async Task<ConversationResponse> GenerateAsync(ConversationRequest request, CancellationToken token)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonDocument>();

        var responseField = jsonResponse?.RootElement.GetProperty("response").GetString() ?? "No response";
        //var context = jsonResponse?.RootElement.GetProperty("context").Deserialize<List<int>>();

        return new ConversationResponse()
        {
            Response = responseField,
          //  Context = context
        };

    }

    public async IAsyncEnumerable<string> StreamAsync(ConversationRequest request, CancellationToken token)
    {
        request.Stream = true;

        var response = await _httpClient.PostAsJsonAsync(
            "/api/generate",
            request,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        response.EnsureSuccessStatusCode(); // Ensure the request was successful

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
    }

    public async Task<IEnumerable<string>> GetAvailableModelsAsync(CancellationToken token)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonDocument>("/api/tags");
        return response?.RootElement.EnumerateArray().Select(m => m.GetString()) ?? new List<string>();
    }

    public async Task EnrichAsync(ConversationRequest request, IEnumerable<IPromptEnricher> enrichers, CancellationToken token)
    {
        PromptContext promptContext = new PromptContext(request);
        foreach (var enricher in enrichers)
        {
            await enricher.EnrichAsync(promptContext);
        }
         
    }
     
}
