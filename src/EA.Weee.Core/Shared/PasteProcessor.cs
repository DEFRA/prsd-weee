namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;

    public class PasteProcessor : IPasteProcessor
    {
        private static readonly string[] NewLineCharactors = { "\r\n", "\r", "\n" };
        private static readonly char[] LineSplitCharactors = { '\t' };

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

        public IList<ObligatedCategoryValue> ParseObligatedPastedValues(ObligatedPastedValues obligatedPastedValues, IList<ObligatedCategoryValue> existingData)
        {
            var categoryValues = (existingData != null) ? existingData : new ObligatedCategoryValues();

            var b2bValuesProvided = obligatedPastedValues.B2B.Any(v => !string.IsNullOrWhiteSpace(v.Tonnage));
            var b2cValuesProvided = obligatedPastedValues.B2C.Any(v => !string.IsNullOrWhiteSpace(v.Tonnage));
            foreach (var category in categoryValues)
            {
                if (b2bValuesProvided)
                {
                    category.B2B = obligatedPastedValues.B2B.Where(o => o.CategoryId == category.CategoryId).FirstOrDefault().Tonnage;
                }

                if (b2cValuesProvided)
                {
                    category.B2C = obligatedPastedValues.B2C.Where(o => o.CategoryId == category.CategoryId).FirstOrDefault().Tonnage;
                }
            }

            return categoryValues;
        }

        public IList<NonObligatedCategoryValue> ParseNonObligatedPastedValues(PastedValues nonObligatedPastedValues, IList<NonObligatedCategoryValue> existingData)
        {
            var categoryValues = (existingData != null) ? existingData : new NonObligatedCategoryValues();

            foreach (var category in categoryValues)
            {
                category.Tonnage = nonObligatedPastedValues.Where(o => o.CategoryId == category.CategoryId).FirstOrDefault().Tonnage;
            }

            return categoryValues;
        }
    }
}
