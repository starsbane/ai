using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Extensions.Downloader;

namespace Starsbane.AI.Google
{
    public sealed class FFmpegSpeechFormatConverter : ISpeechFormatConverter
    {
        private readonly string ffmpegPath;

        public FFmpegSpeechFormatConverter()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            ffmpegPath = Path.Combine(root, "ffmpeg");
            Directory.CreateDirectory(ffmpegPath);

            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = ffmpegPath });
        }

        /// <inheritdoc/>
        public async Task<byte[]>ConvertSpeechFormatAsync(byte[] inputBytes, CancellationToken cancellationToken)
        {
            if (inputBytes == null || inputBytes.Length == 0)
            {
                throw new ArgumentException("Input bytes cannot be null or empty.", nameof(inputBytes));
            }

            var sourceFilePath = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), ".pcm");
            using var sourceFileStream = File.OpenWrite(sourceFilePath);
            await sourceFileStream.WriteAsync(inputBytes, 0, inputBytes.Length, cancellationToken);

            var destinationFilePath = Path.ChangeExtension(sourceFilePath, ".mp3");
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await FFMpegDownloader.DownloadBinaries();

                await FFMpegArguments.FromFileInput(sourceFilePath, true,
                        options => options.WithCustomArgument("-f s16le -ar 24000 -ac 1"))
                    .OutputToFile(
                        destinationFilePath, true,
                        options => options
                            .WithAudioCodec(AudioCodec.LibMp3Lame)
                            .UsingMultithreading(true)
                    )
                    .ProcessAsynchronously();

                using var destinationFileStream = File.OpenRead(destinationFilePath);
                using var ms = new MemoryStream();
                await destinationFileStream.CopyToAsync(ms);

                return ms.ToArray();
            }
            finally
            {
                try
                {
                    File.Delete(sourceFilePath);
                    File.Delete(destinationFilePath);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
