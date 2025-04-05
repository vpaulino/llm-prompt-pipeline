using AIConversationalAgent.ApiModels;
using AIConversationalAgent.Enrichers;
using AIConversationalAgent.Enrichers.Models;
using AIConversationalAgent.ModelAgent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AIConversationalAgent.Controllers
{
    [ApiController]
    [Route("api/llm")]
    public class LLMController : ControllerBase
    {


        private readonly ModelEngineFactory _engineFactory;
        private readonly PipelineEnricherBuilder pipelineEnricherBuilder;
        private readonly EngineGenerationOutputFormatter outputFormatter;
        public LLMController(ModelEngineFactory engineFactory, PipelineEnricherBuilder pipelineEnricherBuilder, EngineGenerationOutputFormatter outputFormatter)
        {
            _engineFactory = engineFactory;
            this.pipelineEnricherBuilder = pipelineEnricherBuilder;
            this.outputFormatter = outputFormatter;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] ConversationRequest request, CancellationToken token)
        {
            Template template = await pipelineEnricherBuilder.BuildAsync(request.Template);
            var engine = await _engineFactory.CreateAsync(request.Engine);

            if (template.Enrichers.Any()) 
            {
                await engine.EnrichAsync(request, template.Enrichers, token);
            }

            var response = await engine.GenerateAsync(request, token);

           

            return Ok(new { status = "Generated", result = response });
        }

        [HttpPost("stream")]
        public async Task<IActionResult> Stream([FromBody] ConversationRequest request, CancellationToken token)
        {
            var engine = await _engineFactory.CreateAsync(request.Engine);
            var responseStream = engine.StreamAsync(request, token);
            return Ok(responseStream);
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetModels([FromQuery] string engine, CancellationToken token)
        {
            var engineInstance = await _engineFactory.CreateAsync(engine);
            var models = await engineInstance.GetAvailableModelsAsync(token);
            return Ok(models);
        }
    }
}
