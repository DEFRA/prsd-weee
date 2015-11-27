namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, string schemaNamespace, SchemaVersion schemaVersion);
    }
}
