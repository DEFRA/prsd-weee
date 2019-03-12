namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;

    public class PasteProcessor : IPasteProcessor
    {
        private static readonly List<string> AllowedCharactors = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "\r", "\n" };
        private static readonly string[] NewLineCharactors = { "\r\n", "\r", "\n" };
        private static readonly char[] LineSplitCharactors = { '\t', ':', ',' };

        public ObligatedCategoryValues BuildModel(ObligatedCategoryValue pasteValues)
        {
            var categoryValues = new ObligatedCategoryValues();

            if (pasteValues == null || string.IsNullOrWhiteSpace(pasteValues.ToString()))
            {
                return categoryValues;
            }

            var B2clines = pasteValues.B2C.ToString().Split(NewLineCharactors, StringSplitOptions.None).ToList();
            var B2blines = pasteValues.B2B.ToString().Split(NewLineCharactors, StringSplitOptions.None).ToList();

            placeholderName(categoryValues, B2clines, true);
            placeholderName(categoryValues, B2blines, false);

            return categoryValues;
        }

        private static void placeholderName(ObligatedCategoryValues categoryValues, List<string> lines, bool isB2c)
        {
            if (lines.Any() && lines.ElementAt(lines.Count() - 1).Equals(string.Empty))
            {
                lines.RemoveAt(lines.Count() - 1);
            }

            for (var lineCount = 0; lineCount < lines.Count; lineCount++)
            {
                var splitLine = lines.ElementAt(lineCount).Split(LineSplitCharactors);

                var category = categoryValues.FirstOrDefault(c => c.CategoryId == lineCount + 1);

                if (category != null)
                {
                    if (splitLine.Length > 0 && !string.IsNullOrWhiteSpace(splitLine[0]))
                    {
                        if (isB2c)
                        {
                            category.B2C = splitLine[0];
                        }
                        else
                        {
                            category.B2B = splitLine[0];
                        }
                    }
                }
            }
        }

        private decimal? TryCastToDecimal(string value)
        {
            decimal output;

            if (decimal.TryParse(value, out output))
            {
                return output;
            }

            return null;
        }
    }
}
