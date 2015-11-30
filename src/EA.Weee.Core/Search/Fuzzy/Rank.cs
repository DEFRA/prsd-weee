namespace EA.Weee.Core.Search.Fuzzy
{
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
