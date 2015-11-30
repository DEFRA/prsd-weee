namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class ProducerSettings : ISettings
    {
        public MemberRegistrationSchemaVersion SchemaVersion { get; set; }

        public bool IsNew { get; set; }

        public string RegistrationNumber { get; set; }

        public bool IgnoreStringLengthConditions { get; set; }

        public bool NoCompaniesForNewProducers { get; set; }
    }
}
