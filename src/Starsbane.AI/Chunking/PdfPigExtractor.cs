using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Util;

namespace Starsbane.AI.Chunking
{
    public class PdfPigExtractor(ISemanticChunker semanticChunker) : IPdfFileChunkExtractor
    {
        public async Task<IEnumerable<Chunk>> ExtractChunksAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var extractedStrings = new List<string>();
            var chunks = new List<Chunk>();

            using var document = PdfDocument.Open(fileName);

            if (document.NumberOfPages > 1)
            {
                var docDecorations = DecorationTextBlockClassifier.Get(document.GetPages().ToList(),
                    DefaultWordExtractor.Instance,
                    DocstrumBoundingBoxes.Instance);
                var _page = 0;
                var bf = "";
                foreach (var page in document.GetPages())
                {
                    // 0. Preprocessing
                    var letters = page.Letters; // no preprocessing

                    // 1. Extract words
                    var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);

                    // 2. Segment page
                    var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);

                    // 3. Postprocessing
                    var orderedTextBlocks = UnsupervisedReadingOrderDetector.Instance.Get(textBlocks);

                    // 4. Extract text, excluding headings & footers
                    foreach (var block in orderedTextBlocks)
                    {
                        var str = block.Text.Normalize(NormalizationForm.FormKC);
                        if (docDecorations[_page].All(x => x.BoundingBox.ToString() != block.BoundingBox.ToString()))
                        {
                            if (str.Split(' ').Length < 10)
                            { //probably headings and titles
                                bf += $" {str}";
                            }
                            else
                            {
                                extractedStrings.Add($"{bf.Replace(Environment.NewLine, " ")} {str.Replace(Environment.NewLine, " ")}");
                                bf = "";
                            }
                        }
                    }
                    _page++;
                }
            }
            else
            {
                extractedStrings.Add(document.GetPage(1).Text);
            }

            foreach (var e in extractedStrings.SkipWhile(string.IsNullOrEmpty))
            {
                chunks.AddRange(await semanticChunker.CreateChunksAsync(e, cancellationToken));
            }

            return chunks;
        }
    }
}
