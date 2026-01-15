namespace Starsbane.AI.Google
{
    public interface ISpeechFormatConverter
    {
        Task<byte[]> ConvertSpeechFormatAsync(byte[] inputBytes, CancellationToken cancellationToken);
    }
}