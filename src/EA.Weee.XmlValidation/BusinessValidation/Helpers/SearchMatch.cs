namespace EA.Weee.XmlValidation.BusinessValidation.Helpers
{
    using System;

    public class SearchMatch : ISearchMatch
    {
        public bool ByProducerName(string pn1, string pn2)
        {
            if (string.IsNullOrEmpty(pn1) || string.IsNullOrEmpty(pn2))
            {
                return pn1 == pn2;
            }

            return string.Equals(pn1.Trim(), pn2.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
