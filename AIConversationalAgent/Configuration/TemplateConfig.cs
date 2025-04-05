namespace AIConversationalAgent.Configuration
{
    public class TemplateConfig
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Enrichers { get; set; } = new();
       
        public List<string> Actions { get; set; } // Default action
        public string Cardinality { get; internal set; }
    }

}
