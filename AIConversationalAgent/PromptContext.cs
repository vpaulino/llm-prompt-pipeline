using AIConversationalAgent.ApiModels;
using System.Collections.Concurrent;

namespace AIConversationalAgent
{
    public class PromptContext
    {
        public ConversationRequest Request { get; }
        public ConcurrentDictionary<string, object> Data { get; } = new();

        public List<int> Context { get; set; }

        public PromptContext(ConversationRequest request)
        {
            Request = request;
        }

        public void Set(string key, object value)
        {
            Data[key] = value;
        }

        public T? Get<T>(string key) where T : class
        {
            return Data.TryGetValue(key, out var value) ? value as T : null;
        }
    }

}
