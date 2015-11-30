namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Terms which appear often in many results may be devalued compared to unique terms.
    /// A relevance of 0 will cause the term to be disregarded.
    /// A relevance of 1 will cause the term to be treated like a unique term.
    /// For example "Ltd" would be a good candidate for a common term when searching company names.
    /// </summary>
    public class CommonTerm
    {
        public ITerm Term { get; private set; }
        public double Relevance { get; private set; }

        public CommonTerm(ITerm term, double relevance)
        {
            if (term == null)
            {
                throw new ArgumentNullException("term");
            }

            if (relevance < 0 || relevance > 1)
            {
                throw new ArgumentOutOfRangeException("relevance");
            }

            Term = term;
            Relevance = relevance;
        }
    }
}
