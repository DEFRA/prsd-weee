namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a term with a unique single value.
    /// </summary>
    public class Word : ITerm
    {
        public string Value { get; private set; }

        public IEnumerable<string> Values
        {
            get { yield return Value; }
        }

        public Word(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value");
            }

            Value = value.Trim();
        }
    }
}
