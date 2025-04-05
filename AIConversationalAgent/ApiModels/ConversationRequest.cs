namespace AIConversationalAgent.ApiModels
{
    public class ConversationRequest
    {
            public string Engine { get; set; } = "ollama"; // Defines which model engine to use
            public string Model { get; set; } = "mistral"; // The model name
            public string Prompt { get; set; } = string.Empty; // The input prompt
            public bool Stream { get; set; } = false; // Whether to stream the response
            public string? Suffix { get; set; } // Optional suffix to append to the response
            public string? Format { get; set; } // Response format (e.g., "json")
            public Dictionary<string, object>? Options { get; set; } // Additional model-specific options
            public string? System { get; set; } // System instructions for context
            public string? Template { get; set; } // Prompt template if applicable
            public bool Raw { get; set; } = false; // Whether to send the prompt as raw text
            public string? KeepAlive { get; set; } // How long to keep the model in memory
       
        public List<int>? Context { get; internal set; }
    }
}