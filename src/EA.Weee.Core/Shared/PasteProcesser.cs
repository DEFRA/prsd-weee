namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;
    using DataReturns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prsd.Core;
    using Prsd.Core.Extensions;

    public class PasteProcesser : IPasteProcesser
    {
        private static readonly List<string> AllowedCharactors = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "\r", "\n" };
        private static readonly string[] NewLineCharactors = { "\r\n", "\r", "\n" };
        private static readonly char[] LineSplitCharactors = { '\t', ':', ',' };

        public NonObligatedCategoryValues BuildModel(object pasteValues)
        {
            var categoryValues = new NonObligatedCategoryValues();

            if (pasteValues == null || string.IsNullOrWhiteSpace(pasteValues.ToString()))
            {
                return categoryValues;
            }

            var lines = pasteValues.ToString().Split(NewLineCharactors, StringSplitOptions.None).ToList();

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
                        category.HouseHold = splitLine[0];
                    }

                    if (splitLine.Length > 1 && !string.IsNullOrWhiteSpace(splitLine[1]))
                    {
                        category.NonHouseHold = splitLine[1];
                    }
                }
            }

            return categoryValues;
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
