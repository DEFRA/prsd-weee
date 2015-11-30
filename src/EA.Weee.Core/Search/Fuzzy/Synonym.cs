namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a term for which there are multiple equivalent values.
    /// For example "Limited" and "Ltd" are synonymous int he context
    /// of company names.
    /// </summary>
    public class Synonym : ITerm
    {
        public IEnumerable<string> Values { get; private set; }

        public Synonym(IEnumerable<string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Any(v => string.IsNullOrWhiteSpace(v)))
            {
                throw new ArgumentException("values");
            }

            Values = values.Select(v => v.Trim());
        }
    }
}
