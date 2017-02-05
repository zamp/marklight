using System.Text;

namespace Marklight
{
    /// <summary>
    /// String parsing utilities.
    /// </summary>
    public static class ParseUtils
    {
        /// <summary>
        /// Parse and separate a set of values in a string which are delimited by either a comma or space.
        /// Typically used for numerical values.
        /// ie. "10, 20px, 30%,5" => { "10", "20px", "30%", "5" }
        /// ie. "10 20px 30% 5" => { "10", "20px", "30%", "5" }
        /// </summary>
        /// <param name="delimStr">The string containing delimited values.</param>
        /// <param name="output">An array to put the parsed values into. The size of the array is the max
        /// number of items to parse.</param>
        /// <param name="startIndex">The optional string index to begin parsing at.</param>
        /// <param name="endIndex">The optional string index to stop parsing at.</param>
        /// <param name="buffer">The optional StringBuilder buffer to use for parsing.</param>
        /// <returns>The number of items parsed. Value will not be larger than the size of the output array.</returns>
        public static int ParseDelimitedValues(string delimStr, string[] output,
                                            int startIndex = -1, int endIndex = -1, StringBuilder buffer = null)
        {

            buffer = buffer ?? new StringBuilder(4);
            buffer.Length = 0;

            startIndex = startIndex == -1 ? 0 : startIndex;
            endIndex = endIndex == -1 ? delimStr.Length - 1 : endIndex;

            var isStarted = false;
            var outputIndex = 0;

            for (var i = startIndex; i <= endIndex; i++)
            {
                var ch = delimStr[i];
                if (", \r\n\t".IndexOf(ch) != -1)
                {
                    if (isStarted)
                    {
                        isStarted = false;
                        output[outputIndex] = buffer.ToString();
                        buffer.Length = 0;
                        outputIndex++;
                        if (outputIndex > output.Length)
                            break;
                    }
                    continue;
                }

                buffer.Append(ch);
                isStarted = true;
            }

            if (buffer.Length == 0)
                return outputIndex;

            output[outputIndex] = buffer.ToString();
            outputIndex++;

            return outputIndex;
        }

    }
}