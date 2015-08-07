namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public interface ISettings
    {
        SchemaVersion SchemaVersion { get; }
        bool IgnoreStringLengthConditions { get; }
    }
}
