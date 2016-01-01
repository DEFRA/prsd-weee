namespace EA.Weee.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A formatter that sanitizes input by replacing any double quotes with single quotes and
    /// encodes input by wrapping all values in double quotes.
    /// </summary>
    public class DefaultFormatter : IIbisFormatter
    {
        private const string doubleQuotes = "\"";
        private const string singleQuote = "'";

        public virtual string Format(string input)
        {
            string sanitizedInput = input.Replace(doubleQuotes, singleQuote);

            return string.Format("{0}{1}{2}", doubleQuotes, sanitizedInput, doubleQuotes);
        }
    }
}
