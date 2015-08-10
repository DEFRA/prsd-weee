namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class ProducerSettings : IProducerBusinessSettings, IAuthorizedRepresentativeSettings
    {
        public SchemaVersion SchemaVersion { get; set; }

        public bool IsNew { get; set; }

        public string RegistrationNumber { get; set; }

        public bool IgnoreStringLengthConditions { get; set; }
    }
}
