namespace AIConversationalAgent.Enrichers.Models
{
    public class Template
    {
        public string Name { get; set; }

        public IEnumerable<IPromptEnricher> Enrichers { get; set; }

        public string OutputFormat { get; set; }

        public string Cardinality { get; set; }

        public IEnumerable<string> Actions { get; set; }
    }
}
