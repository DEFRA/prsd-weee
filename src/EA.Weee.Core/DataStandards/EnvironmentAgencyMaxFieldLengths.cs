namespace EA.Weee.Core.DataStandards
{
    public static class EnvironmentAgencyMaxFieldLengths
    {
        public const int CompanyRegistrationNumber = 10;
        public const int SchemeName = 70;
        public const int SchemeApprovalNumber = 16;
        public const int IbisBillingReference = 10;
        public const int ProducerRegistrationNumber = 50;
        public const int ProducerSearchTerm = CommonMaxFieldLengths.DefaultString + ProducerRegistrationNumber + 2; // +2 as search term is comma separated e.g. MyProducer, MY1234PRN
    }
}
