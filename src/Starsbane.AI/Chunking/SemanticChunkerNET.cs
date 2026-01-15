// The implementation of class SemanticChunkerNET is from https://github.com/GregorBiswanger/SemanticChunker.NET

using ICU4N.Text;
using System.Collections.Immutable;
using Microsoft.Extensions.AI;
// ReSharper disable UseIndexFromEndExpression

namespace Starsbane.AI.Chunking
{
    /// <summary>
    /// Splits long text into semantically coherent chunks based on sentence embeddings
    /// and a configurable breakpoint threshold.
    /// </summary>
    /// <param name="embeddingGenerator">
    ///    Service that converts a string into an <see cref="Embedding"/>.
    /// </param>
    /// <param name="tokenLimit">
    ///    Maximum large language model token budget for a single chunk.
    ///    A safety margin of ten percent is subtracted automatically.
    /// </param>
    /// <param name="bufferSize">
    ///    Number of surrounding sentences taken into account when building the
    ///    contextual sentence windows from which embeddings are created.
    /// </param>
    /// <param name="thresholdType">
    ///    Statistical method that determines the breakpoint threshold.
    /// </param>
    /// <param name="thresholdAmount">
    ///    Optional override for the chosen <paramref name="thresholdType"/>.  
    ///    If omitted, a well‑established default is used.
    /// </param>
    /// <param name="targetChunkCount">
    ///    Desired number of chunks.  
    ///    If specified, the threshold is derived from this value instead of the
    ///    methods defined by <paramref name="thresholdType"/> and
    ///    <paramref name="thresholdAmount"/>.
    /// </param>
    /// <param name="minChunkChars">
    ///    Chunks shorter than this value are skipped entirely.
    /// </param>
    public sealed class SemanticChunkerNET(
        IAIClient client,
        int bufferSize = 1,
        BreakpointThresholdType thresholdType = BreakpointThresholdType.Percentile,
        double? thresholdAmount = null,
        int? targetChunkCount = null,
        int minChunkChars = 0) : ISemanticChunker
    {
        private static readonly IReadOnlyDictionary<BreakpointThresholdType, double> DefaultThresholdAmounts =
               new Dictionary<BreakpointThresholdType, double>
               {
            { BreakpointThresholdType.Percentile,        95 },
            { BreakpointThresholdType.StandardDeviation, 3  },
            { BreakpointThresholdType.InterQuartile,     1.5},
            { BreakpointThresholdType.Gradient,          95 }
               }.ToImmutableDictionary();

        private readonly IAIEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = client.GetEmbeddingGenerator();
        private int MaximumChunkCharacters => (int)(_embeddingGenerator.EmbeddingTokenLimit * 4 * 0.9);

        /// <summary>
        /// Creates semantic chunks from the supplied <paramref name="text"/>.
        /// </summary>
        /// <param name="text">Full‑length input text.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// List of <see cref="Chunk"/> instances that together cover the entire text.
        /// </returns>
        public async Task<IEnumerable<Chunk>> CreateChunksAsync(string text, CancellationToken cancellationToken = default)
        {
            IList<string> sentences = SplitIntoSentences(text);

            if (sentences.Count <= 1)
            {
                return
                [
                    new Chunk
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = text,
                    Embedding = await _embeddingGenerator.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken)
                }
                ];
            }

            IList<string> contextualSentences = BuildContextualSentences(sentences, bufferSize);

            var contextualEmbeddings = await Task.WhenAll(contextualSentences.Select(s => _embeddingGenerator.GenerateEmbeddingAsync(s, cancellationToken: cancellationToken)));

            var distances = CalculateSentenceDistances(contextualEmbeddings);

            var breakpointThreshold = targetChunkCount.HasValue
                ? ThresholdFromTargetCount(distances, targetChunkCount.Value)
                : CalculateThreshold(distances, thresholdType, thresholdAmount ?? DefaultThresholdAmounts[thresholdType]);

            var breakpoints = new HashSet<int>(distances
                .Select((distance, index) => (distance, index))
                .Where(t => t.distance > breakpointThreshold)
                .Select(t => t.index));

            return await AssembleChunksAsync(sentences, breakpoints, minChunkChars, cancellationToken);
        }

        /// <summary>
        /// Calculates the cosine similarity between two vectors of equal length.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <returns>
        /// A value between -1 and1 representing the cosine similarity:
        ///1 indicates identical direction,0 indicates orthogonality, and -1 indicates opposite direction.
        /// Returns0 if either vector has zero magnitude.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="vectorA"/> or <paramref name="vectorB"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the vectors do not have the same length.
        /// </exception>
        public static double CosineSimilarity(IReadOnlyList<float> vectorA, IReadOnlyList<float> vectorB)
        {
            if (vectorA is null) throw new ArgumentNullException(nameof(vectorA));
            if (vectorB is null) throw new ArgumentNullException(nameof(vectorB));
            if (vectorA.Count != vectorB.Count) throw new ArgumentException("Vectors must have the same length.");

            double dot = 0, na = 0, nb = 0;

            for (var i = 0; i < vectorA.Count; i++)
            {
                var ai = vectorA[i];
                var bi = vectorB[i];
                dot += ai * bi;
                na += ai * ai;
                nb += bi * bi;
            }

            var denom = Math.Sqrt(na) * Math.Sqrt(nb);
            if (denom == 0) return 0;

            var cos = dot / denom;

            if (cos > 1) return 1;
            if (cos < -1) return -1;

            return cos;
        }

        private static IList<string> SplitIntoSentences(string text)
        {
            var result = new List<string>();
            BreakIterator iterator = BreakIterator.GetSentenceInstance();
            iterator.SetText(text);

            for (int start = iterator.First(), end = iterator.Next();
                 end != BreakIterator.Done;
                 start = end, end = iterator.Next())
            {
                var sentence = text.Substring(start, end - start).Trim();
                if (sentence.Length > 0)
                {
                    result.Add(sentence);
                }
            }

            return result;
        }

        private static IList<string> BuildContextualSentences(IList<string> sentences, int buffer)
        {
            var result = new List<string>(sentences.Count);

            for (var i = 0; i < sentences.Count; i++)
            {
                var contextBefore = sentences.Skip(Math.Max(0, i - buffer)).Take(buffer);
                var contextAfter = sentences.Skip(i + 1).Take(buffer);

                result.Add(string.Join(" ", contextBefore.Concat([sentences[i]]).Concat(contextAfter)));
            }

            return result;
        }

        private static double[] CalculateSentenceDistances(IReadOnlyList<Embedding<float>> embeddings)
        {
            var distances = new double[embeddings.Count - 1];

            for (var i = 0; i < distances.Length; i++)
            {
                var vectorA = embeddings[i].Vector.ToArray();
                var vectorB = embeddings[i + 1].Vector.ToArray();

                distances[i] = 1d - CosineSimilarity(vectorA, vectorB);
            }

            return distances;
        }

        private async Task<IList<Chunk>> AssembleChunksAsync(
            IList<string> sentences,
            HashSet<int> breakpoints,
            int minimumCharacters,
            CancellationToken cancellationToken)
        {
            var chunks = new List<Chunk>();
            var currentSentences = new List<string>();

            for (var index = 0; index < sentences.Count; index++)
            {
                currentSentences.Add(sentences[index]);

                var isBreakpoint = breakpoints.Contains(index) || index == sentences.Count - 1;
                if (!isBreakpoint) continue;

                var chunkText = string.Join(" ", currentSentences).Trim();

                if (chunkText.Length < minimumCharacters)
                {
                    currentSentences.Clear();
                    continue;
                }

                if (chunkText.Length > MaximumChunkCharacters)
                {
                    chunkText = chunkText.Substring(0, MaximumChunkCharacters);
                }

                var embedding = await _embeddingGenerator.GenerateEmbeddingAsync(chunkText, cancellationToken: cancellationToken);

                if (embedding != null)
                {
                    chunks.Add(new Chunk
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = chunkText,
                        Embedding = embedding
                    });
                }

                currentSentences.Clear();
            }

            return chunks;
        }

        private static double CalculateThreshold(
            IReadOnlyList<double> distances,
            BreakpointThresholdType type,
            double amount)
        {
            return type switch
            {
                BreakpointThresholdType.Percentile =>
                    Percentile(distances, amount),

                BreakpointThresholdType.StandardDeviation =>
                    distances.Average() + amount * StandardDeviation(distances),

                BreakpointThresholdType.InterQuartile =>
                    distances.Average() + amount *
                    (Percentile(distances, 75) - Percentile(distances, 25)),

                BreakpointThresholdType.Gradient =>
                    Percentile(Gradient(distances), amount),

                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        private static double ThresholdFromTargetCount(IReadOnlyList<double> distances, int desiredChunks)
        {
            var maxChunks = distances.Count;
            var minChunks = 1;

            var clampedChunks = Math.Min(Math.Max(desiredChunks, minChunks), maxChunks);

            double y1 = 0;   // percentile for maxChunks
            double y2 = 100; // percentile for minChunks

            var percentile = maxChunks == minChunks
                ? y2
                : y1 + (y2 - y1) * (clampedChunks - maxChunks) / (minChunks - maxChunks);

            return Percentile(distances, percentile);
        }

        private static double Percentile(IReadOnlyList<double> sequence, double p)
        {
            var sorted = sequence.OrderBy(v => v).ToArray();
            var n = (sorted.Length - 1) * p / 100d;
            var k = (int)Math.Floor(n);
            var d = n - k;

            return k + 1 < sorted.Length
                ? sorted[k] + d * (sorted[k + 1] - sorted[k])
                : sorted[sorted.Length - 1];
        }

        private static double StandardDeviation(IReadOnlyList<double> sequence)
        {
            var average = sequence.Average();
            var variance = sequence.Sum(v => Math.Pow(v - average, 2)) / sequence.Count;
            return Math.Sqrt(variance);
        }

        private static double[] Gradient(IReadOnlyList<double> sequence)
        {
            var g = new double[sequence.Count];

            for (var i = 1; i < sequence.Count - 1; i++)
            {
                g[i] = (sequence[i + 1] - sequence[i - 1]) / 2d;
            }

            g[0] = sequence[1] - sequence[0];
            g[g.Length - 1] = sequence[sequence.Count - 1] - sequence[sequence.Count - 2];

            return g;
        }
    }
}
