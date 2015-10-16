namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A word provided as part of a search phrase.
    /// 
    /// The IsPartial property may be used for as-you-type searching,
    /// where the current word being entered by the user may be incomplete.
    /// </summary>
    public class SearchWord
    {
        public string Value { get; private set; }
        public bool IsPartial { get; private set; }

        public SearchWord(string value, bool isPartial)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value");
            }

            Value = value.Trim();
            IsPartial = isPartial;
        }
    }
}
