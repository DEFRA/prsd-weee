namespace EA.Weee.Core.Search.Fuzzy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Rank
    {
        public double Confidence { get; private set; }
        public double Relevance { get; private set; }

        public Rank(double confidence, double relevance)
        {
            Confidence = confidence;
            Relevance = relevance;
        }
    }
}
