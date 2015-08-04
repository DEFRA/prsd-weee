namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public interface ISettings
    {
        SchemaVersion SchemaVersion { get; }
        bool IgnoreStringLengthConditions { get; }
    }
}
