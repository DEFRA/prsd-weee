namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A term within a search result.
    /// The relevance defines how important this term is relative to other terms
    /// in search results. A higher value means a matching term in a search phrase
    /// is more likely to yiled this result.
    /// </summary>
    public class ResultTerm
    {
        public ITerm Term { get; private set; }
        public double Relevance { get; private set; }

        public ResultTerm(ITerm term, double relevance)
        {
            if (term == null)
            {
                throw new ArgumentNullException("term");
            }

            Term = term;
            Relevance = relevance;
        }
    }
}
