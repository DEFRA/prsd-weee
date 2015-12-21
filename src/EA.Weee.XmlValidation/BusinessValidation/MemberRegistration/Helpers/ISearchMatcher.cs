namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Helpers
{
    public interface ISearchMatcher
    {
        bool MatchByProducerName(string producerName1, string producerName2);
    }
}
