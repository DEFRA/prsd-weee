namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Prsd.Core;

    public class PasteProcessor : IPasteProcessor
    {
        private static readonly List<string> AllowedCharacters = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "\r", "\n" };
        private static readonly string[] NewLineCharactors = { "\r\n", "\r", "\n" };
        private static readonly char[] LineSplitCharactors = { '\t', ':', ',' };

        public PastedValues BuildModel(string pasteValues)
        {
            var categoryValues = new PastedValues();

            if (pasteValues == null || string.IsNullOrWhiteSpace(pasteValues.ToString()))
            {
                return categoryValues;
            }

            var lines = pasteValues.ToString().Split(NewLineCharactors, StringSplitOptions.None).Take(categoryValues.Count).ToList();

            ParseTonnageValues(categoryValues, lines);

            return categoryValues;
        }

        private static void ParseTonnageValues(PastedValues categoryValues, List<string> lines)
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
                        category.Tonnage = splitLine[0];
                    }
                }
            }
        }

        public ObligatedCategoryValues ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues)
        {
            var obligatedCategoryValues = new ObligatedCategoryValues();

            foreach (var category in obligatedCategoryValues)
            {
                category.B2B = obligatedPastedValues.B2B.Where(o => o.CategoryId == category.CategoryId).FirstOrDefault().Tonnage;
                category.B2C = obligatedPastedValues.B2C.Where(o => o.CategoryId == category.CategoryId).FirstOrDefault().Tonnage;
            }

            return obligatedCategoryValues;
        }
    }
}
