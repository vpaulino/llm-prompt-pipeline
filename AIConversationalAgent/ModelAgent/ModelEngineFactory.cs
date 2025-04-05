namespace AIConversationalAgent.ModelAgent
{
    public class ModelEngineFactory 
    {
        private readonly IServiceProvider _serviceProvider;

        public ModelEngineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IModelEngineService> CreateAsync(string engine)
        {
            return engine.ToLower() switch
            {
                "ollama" => _serviceProvider.GetKeyedService<IModelEngineService>(engine),
                _ => throw new NotImplementedException($"Engine '{engine}' is not supported.")
            };
        }
    }

}
