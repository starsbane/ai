namespace Starsbane.AI
{
    public interface IOCRClient
    {
        Task<string> ExtractFromImage(BinaryData imageData, TextExtractionOptions? options = null, CancellationToken cancellationToken = new CancellationToken());
    }
}
