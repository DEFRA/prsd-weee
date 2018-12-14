namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;
    using Prsd.Core;
    using Prsd.Core.Extensions;

    public class PasteProcesser : IPasteProcesser
    {
        private static readonly List<string> AllowedCharactors = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "\r", "\n" };
        private static readonly string[] NewLineCharactors = { "\r\n", "\r", "\n" };
        private static readonly char[] LineSplitCharactors = { '\t' };

        public CategoryValues BuildModel(object pasteValues)
        {
            var categoryValues = new CategoryValues();

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

                var category = categoryValues.FirstOrDefault(c => c.Category == (WeeeCategory)lineCount + 1);

                if (category != null)
                {
                    if (splitLine.Length > 0 && !string.IsNullOrWhiteSpace(splitLine[0]))
                    {
                        category.HouseHold = Convert.ToDecimal(splitLine[0]);
                    }

                    if (splitLine.Length > 1 && !string.IsNullOrWhiteSpace(splitLine[1]))
                    {
                        category.NonHouseHold = Convert.ToDecimal(splitLine[1]);
                    }
                }
            }

            return categoryValues;
        }
        //private string StripCharactors(string line)
        //{

        //    var allowed = line.Select(c => AllowedCharactors.Contains(c.ToString()) ? c : ' ');

        //    return new string(allowed.ToArray()).Replace(" ", string.Empty);
        //}
    }
}
