namespace Starsbane.AI
{
    public interface IFileChunkExtractor
    {
        Task<IEnumerable<Chunk>> ExtractChunksAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
