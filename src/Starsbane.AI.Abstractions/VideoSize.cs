using System.Drawing;
using static System.Text.RegularExpressions.Regex;

namespace Starsbane.AI
{
    public static class VideoSize
    {
        /// <summary>
        /// Get Size object according to input string.
        /// </summary>
        /// <param name="sizeString">Size string. Must be one of the following format: XXXp, XXX*YYY or AAAxBBB.</param>
        /// <returns>Size object.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Size Get(string sizeString)
        {
            if (Match(sizeString, "^([0-9]+)p$") is { Success: true } match1 && int.TryParse(match1.Groups[1].Value, out var height))
            {
                return new Size(height / 3 * 4, height);
            }
            else if (Match(sizeString, "^([0-9]+)[*x]([0-9]+)$") is { Success: true } match2 && int.TryParse(match2.Groups[1].Value, out var width) && int.TryParse(match2.Groups[2].Value, out height))

            {
                return new Size(width, height);
            }

            throw new ArgumentNullException($"Invalid format of size");
        }
    }
}
