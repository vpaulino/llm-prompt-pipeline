namespace AIConversationalAgent.Configuration
{
    public class ModelSettings
    {
        public List<ModelConfig> Models { get; set; } = new();
        public List<TemplateConfig>   Templates { get; set; }


    }

    public class ModelConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

}
