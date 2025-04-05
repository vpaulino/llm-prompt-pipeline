using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Configuration;
using AIConversationalAgent.Enrichers.Models;

namespace AIConversationalAgent.Enrichers
{
    public class PipelineEnricherBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, TemplateConfig> _templates;

        public PipelineEnricherBuilder(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _templates = config.GetSection("llm").GetSection("Templates").Get<List<TemplateConfig>>()?
                          .ToDictionary(t => t.Name, t => t) ?? new();
        }

        public async Task<Template> BuildAsync(string templateName)
        {
            if (!_templates.TryGetValue(templateName, out var templateConfig))
            {
                throw new ArgumentException($"Unknown template: {templateName}");
            }

            var enrichers = templateConfig.Enrichers
                .Select(name => _serviceProvider.GetKeyedService<IPromptEnricher>(name))
                .Where(e => e != null)
                .ToList();

            return new Template
            {
                Actions = templateConfig.Actions,
                Cardinality = templateConfig.Cardinality,
                Enrichers = enrichers,
                Name = templateConfig.Name,
                OutputFormat = templateConfig.OutputFormat,

            };  
        }
    }

    public class TemplateResult
    {
        public ConversationRequest FinalPrompt { get; set; }
        public string OutputFormat { get; set; }
        public string Action { get; set; }
    }

}
