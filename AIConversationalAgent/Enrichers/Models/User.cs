
namespace AIConversationalAgent.Enrichers.Models
{
    public class User
    {
        public object Name { get; internal set; }
        public object Email { get; internal set; }
        public int Id { get; internal set; }
        public List<string> Topics { get; internal set; }
    }
}