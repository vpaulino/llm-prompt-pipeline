using AIConversationalAgent;
using AIConversationalAgent.Enrichers;
using AIConversationalAgent.Enrichers.DataProviders;
using AIConversationalAgent.ModelAgent;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Register services
builder.Services.AddHttpClient<IModelEngineService, OllamaEngineService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5); // Increase timeout to 5 minutes
});

builder.Services.AddScoped<IUserDataProvider, InMemoryUserDataProvider>();
builder.Services.AddScoped<IEventDataProvider, InMemoryEventDataProvider>();

builder.Services.AddScoped<EngineGenerationOutputFormatter>();
builder.Services.AddKeyedScoped<IPromptEnricher, TranslateToenGBEnricher>("TranslateToenGBEnricher");
builder.Services.AddKeyedScoped<IPromptEnricher, PromptNormalizer>("PromptNormalizer"); 
builder.Services.AddKeyedScoped<IPromptEnricher, EventMetadataEnricher>("EventMetadataEnricher");
builder.Services.AddKeyedScoped<IPromptEnricher, UserEnricher>("UserEnricher");
builder.Services.AddKeyedScoped<IPromptEnricher, ScopeExtractorEnricher>("ScopeExtractorEnricher");
builder.Services.AddScoped<PipelineEnricherBuilder>();
builder.Services.AddScoped<ModelEngineFactory>();
builder.Services.AddKeyedScoped<IModelEngineService, OllamaEngineService>("ollama");

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5); // Increase connection timeout
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5); // Increase header timeout
    serverOptions.Limits.MaxRequestBodySize = 524288000; // Allow large requests (500MB)
});

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
