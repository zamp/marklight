using System.Text;

namespace MarkLight
{
    /// <summary>
    /// String parsing utilities.
    /// </summary>
    public static class ParseUtils
    {
        /// <summary>
        /// Parse and separate a set of values in a string which are delimited by either a comma or space.
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
        public static int ParseDelimitedStrings(string delimStr, string[] output,
                                            int startIndex = -1, int endIndex = -1, StringBuilder buffer = null)
        {
            buffer = buffer ?? new StringBuilder(6);
            buffer.Length = 0;

            startIndex = startIndex == -1 ? 0 : startIndex;
            endIndex = endIndex == -1 ? delimStr.Length - 1 : endIndex;

            var isValueParsed = false;
            var outputIndex = 0;

            for (var i = startIndex; i <= endIndex; i++)
            {
                var ch = delimStr[i];

                // check for delimiter characters
                if (", \r\n\t".IndexOf(ch) != -1)
                {
                    // check if we have parsed a value that is ready to be added to
                    // the results.
                    if (isValueParsed)
                    {
                        isValueParsed = false;
                        output[outputIndex] = buffer.ToString();
                        buffer.Length = 0;
                        outputIndex++;
                        if (outputIndex > output.Length)
                            break;
                    }
                    continue;
                }

                buffer.Append(ch);
                isValueParsed = true;
            }

            if (buffer.Length == 0)
                return outputIndex;

            // make sure anything still in the buffer is flushed to output
            output[outputIndex] = buffer.ToString();
            outputIndex++;

            return outputIndex;
        }

        /// <summary>
        /// Parse and separate a set of values in a string which are delimited by either a comma or space.
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
        public static int ParseDelimitedNumbers(string delimStr, ParsedNumber[] output,
                                                int startIndex = -1, int endIndex = -1, StringBuilder buffer = null)
        {
            buffer = buffer ?? new StringBuilder(6);
            buffer.Length = 0;

            startIndex = startIndex == -1 ? 0 : startIndex;
            endIndex = endIndex == -1 ? delimStr.Length - 1 : endIndex;

            var isValueParsed = false;
            var isUnits = false;
            var outputIndex = 0;
            var currentValue = new ParsedNumber();

            for (var i = startIndex; i <= endIndex; i++)
            {
                var ch = delimStr[i];

                // check for delimiter characters
                if (", \r\n\t".IndexOf(ch) != -1)
                {
                    // check if we have parsed a value that is ready to be added to
                    // the results.
                    if (isValueParsed)
                    {
                        // Add buffer to result
                        if (isUnits)
                        {
                            currentValue.Unit = buffer.ToString();
                        }
                        else
                        {
                            currentValue.Number = buffer.ToString();
                        }

                        // reset state
                        isValueParsed = false;
                        isUnits = false;

                        // add current value to output and reset
                        output[outputIndex] = currentValue;
                        currentValue = new ParsedNumber();

                        buffer.Length = 0;
                        outputIndex++;

                        if (outputIndex > output.Length)
                            break;
                    }

                    continue;
                }

                // switch to parsing units once a non-number character is parsed.
                if (!isUnits && "+-.0123456789".IndexOf(ch) == -1)
                {
                    currentValue.Number = buffer.ToString();
                    buffer.Length = 0;
                    isUnits = true;
                }

                buffer.Append(ch);
                isValueParsed = true;
            }

            if (buffer.Length == 0)
                return outputIndex;

            // make sure anything still in the buffer is flushed to output
            if (isUnits)
            {
                currentValue.Unit = buffer.ToString();
            }
            else
            {
                currentValue.Number = buffer.ToString();
            }

            output[outputIndex] = currentValue;
            outputIndex++;

            return outputIndex;
        }
    }
}