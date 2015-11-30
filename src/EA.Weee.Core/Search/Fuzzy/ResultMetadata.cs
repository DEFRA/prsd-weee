namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides metadata about a search result.
    /// Metadata for a result may be calculated once and added to
    /// the search result to improve performance next time the search result
    /// is processed.
    /// </summary>
    public class ResultMetadata
    {
        public IList<ResultTerm> ResultTerms { get; private set; }

        public ResultMetadata(IList<ResultTerm> resultTerms)
        {
            if (resultTerms == null)
            {
                throw new ArgumentNullException("resultTerms");
            }

            if (resultTerms.Any(rt => rt == null))
            {
                throw new ArgumentException("resultTerms");
            }

            ResultTerms = resultTerms;
        }
    }
}
