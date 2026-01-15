namespace Starsbane.AI
{
    public interface ISemanticChunker
    {
        Task<IEnumerable<Chunk>> CreateChunksAsync(string text, CancellationToken cancellationToken = default);
    }
}
