namespace EA.Weee.XmlValidation.BusinessValidation.Helpers
{
    using System;

    public class SearchMatcher : ISearchMatcher
    {
        public bool MatchByProducerName(string producerName1, string producerName2)
        {
            if (string.IsNullOrEmpty(producerName1) || string.IsNullOrEmpty(producerName2))
            {
                return producerName1 == producerName2;
            }

            return string.Equals(producerName1.Trim(), producerName2.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
