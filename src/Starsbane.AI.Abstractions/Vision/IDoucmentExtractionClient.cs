namespace Starsbane.AI
{
    public interface IDoucmentExtractionClient
    {
        Task<string> ExtractFromDocument(BinaryData documentData, DocumentExtractionOptions? options = null, CancellationToken cancellationToken = new CancellationToken());
    }
}
