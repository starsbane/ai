using System.Diagnostics.CodeAnalysis;



namespace Starsbane.AI
{
    public static class SpeechGeneratorExtensions
    {
        private static readonly Dictionary<string, string> _extensionToMimeType = new(StringComparer.OrdinalIgnoreCase)
        {
            [".mp3"] = "audio/mp3",
            [".pcm"] = "audio/pcm",
            [".ogg"] = "audio/ogg",
        };

        /// <summary>Asks the <see cref="ISpeechGenerator"/> for an object of type <typeparamref name="TService"/>.</summary>
        /// <typeparam name="TService">The type of the object to be retrieved.</typeparam>
        /// <param name="generator">The generator.</param>
        /// <param name="serviceKey">An optional key that can be used to help identify the target service.</param>
        /// <returns>The found object, otherwise <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The purpose of this method is to allow for the retrieval of strongly typed services that may be provided by the <see cref="ISpeechGenerator"/>,
        /// including itself or any services it might be wrapping.
        /// </remarks>
        public static TService? GetService<TService>(this ISpeechGenerator generator, object? serviceKey = null)
        {
            _ = Throw.IfNull(generator);

            return generator.GetService(typeof(TService), serviceKey) is TService service ? service : default;
        }

        /// <summary>
        /// Asks the <see cref="ISpeechGenerator"/> for an object of the specified type <paramref name="serviceType"/>
        /// and throws an exception if one isn't available.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="serviceType">The type of object being requested.</param>
        /// <param name="serviceKey">An optional key that can be used to help identify the target service.</param>
        /// <returns>The found object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">No service of the requested type for the specified key is available.</exception>
        /// <remarks>
        /// The purpose of this method is to allow for the retrieval of services that are required to be provided by the <see cref="ISpeechGenerator"/>,
        /// including itself or any services it might be wrapping.
        /// </remarks>
        public static object GetRequiredService(this ISpeechGenerator generator, Type serviceType, object? serviceKey = null)
        {
            _ = Throw.IfNull(generator);
            _ = Throw.IfNull(serviceType);

            return
                generator.GetService(serviceType, serviceKey) ??
                throw Throw.CreateMissingServiceException(serviceType, serviceKey);
        }

        /// <summary>
        /// Asks the <see cref="ISpeechGenerator"/> for an object of type <typeparamref name="TService"/>
        /// and throws an exception if one isn't available.
        /// </summary>
        /// <typeparam name="TService">The type of the object to be retrieved.</typeparam>
        /// <param name="generator">The generator.</param>
        /// <param name="serviceKey">An optional key that can be used to help identify the target service.</param>
        /// <returns>The found object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">No service of the requested type for the specified key is available.</exception>
        /// <remarks>
        /// The purpose of this method is to allow for the retrieval of strongly typed services that are required to be provided by the <see cref="ISpeechGenerator"/>,
        /// including itself or any services it might be wrapping.
        /// </remarks>
        public static TService GetRequiredService<TService>(this ISpeechGenerator generator, object? serviceKey = null)
        {
            _ = Throw.IfNull(generator);

            if (generator.GetService(typeof(TService), serviceKey) is not TService service)
            {
                throw Throw.CreateMissingServiceException(typeof(TService), serviceKey);
            }

            return service;
        }

        /// <summary>
        /// Generates speechs based on a text prompt.
        /// </summary>
        /// <param name="generator">The speech generator.</param>
        /// <param name="prompt">The prompt to guide the speech generation.</param>
        /// <param name="options">The speech generation options to configure the request.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> or <paramref name="prompt"/> is <see langword="null"/>.</exception>
        /// <returns>The speechs generated by the generator.</returns>
        public static Task<SpeechGenerationResponse> GenerateSpeechAsync(
            this ISpeechGenerator generator,
            string prompt,
            SpeechGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            _ = Throw.IfNull(generator);
            _ = Throw.IfNull(prompt);

            return generator.GenerateAsync(new SpeechGenerationRequest(prompt), options, cancellationToken);
        }

        /// <summary>
        /// Gets the media type based on the file extension.
        /// </summary>
        /// <param name="fileName">The filename to extract the media type from.</param>
        /// <returns>The inferred media type.</returns>
        private static string GetMediaTypeFromFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (_extensionToMimeType.TryGetValue(extension, out string? mediaType))
            {
                return mediaType;
            }

            return "audio/mp3"; // Default to MP4 if unknown extension
        }
    }
}
