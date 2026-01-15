using System.Text.RegularExpressions;

namespace Starsbane.AI.Amazon
{
    internal static class S3UriParser
    {
        public static (string Bucket, string Key) ParseS3Uri(string s3Uri)
        {
            // Regex to match both virtual-hosted style (bucket.s3.amazonaws.com) 
            // and path-style (s3.amazonaws.com) URIs.
            var regex = new Regex(@"https?:\/\/(?<bucket>.*?)\.s3[\s\S]*?amazonaws\.com\/(?<key>.*)|https?:\/\/s3[\s\S]*?amazonaws\.com\/(?<bucket>.*?)\/(?<key>.*)");
            var match = regex.Match(s3Uri);

            if (match.Success)
            {
                return (match.Groups["bucket"].Value, match.Groups["key"].Value);
            }

            // Handle s3:// scheme if necessary (simple parsing)
            if (s3Uri.StartsWith("s3://"))
            {
                var parts = s3Uri.Substring(5).Split('/');
                return (parts[0], string.Join("/", parts, 1, parts.Length - 1));
            }

            throw new ArgumentException("Invalid S3 URI format", nameof(s3Uri));
        }
    }
}
