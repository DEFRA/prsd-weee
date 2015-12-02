namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public interface ISettings
    {
        MemberRegistrationSchemaVersion SchemaVersion { get; }
        bool IgnoreStringLengthConditions { get; }
    }
}
