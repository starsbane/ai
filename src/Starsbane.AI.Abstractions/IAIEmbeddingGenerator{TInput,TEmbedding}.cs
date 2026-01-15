using Microsoft.Extensions.AI;

namespace Starsbane.AI
{
    /// <summary>Represents a generator of embeddings.</summary>
    /// <typeparam name="TInput">The type from which embeddings will be generated.</typeparam>
    /// <typeparam name="TEmbedding">The type of embeddings to generate.</typeparam>
    /// <remarks>
    /// <para>
    /// Unless otherwise specified, all members of <see cref="IAIEmbeddingGenerator{TInput, TEmbedding}"/> are thread-safe for concurrent use.
    /// It is expected that all implementations of <see cref="IAIEmbeddingGenerator{TInput, TEmbedding}"/> support being used by multiple requests concurrently.
    /// Instances must not be disposed of while the instance is still in use.
    /// </para>
    /// <para>
    /// However, implementations of <see cref="IAIEmbeddingGenerator{TInput, TEmbedding}"/> may mutate the arguments supplied to
    /// <see cref="IAIEmbeddingGenerator{TInput, TEmbedding}.GenerateAsync"/> and <see cref="IAIEmbeddingGenerator{TInput, TEmbedding}.GenerateAsync"/>, such as by configuring the options instance. Thus, consumers of the interface either should
    /// avoid using shared instances of these arguments for concurrent invocations or should otherwise ensure by construction that
    /// no <see cref="IEmbeddingGenerator{TInput, TEmbedding}"/> instances are used which might employ such mutation.
    /// </para>
    /// </remarks>
    /// <related type="Article" href="https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai#the-iembeddinggenerator-interface">The IEmbeddingGenerator interface.</related>
    public interface IAIEmbeddingGenerator<in TInput, TEmbedding> : IEmbeddingGenerator<TInput, TEmbedding>
        where TEmbedding : Embedding
    {
        /// <summary>Generates embedding for the supplied <paramref name="value"/>.</summary>
        /// <param name="value">The value for which to generate embedding.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <related type="Article" href="https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai#create-embeddings">Create embeddings.</related>
        Task<TEmbedding> GenerateEmbeddingAsync(string value, Microsoft.Extensions.AI.EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Read-only property that indicates the maximum number of tokens that can be sent in a single embedding request.
        /// </summary>
        int EmbeddingTokenLimit { get; }
    }
}
