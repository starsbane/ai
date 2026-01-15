using Microsoft.Extensions.AI;

namespace Starsbane.AI
{
    public class Chunk
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public Embedding<float> Embedding { get; set; }
    }
}
