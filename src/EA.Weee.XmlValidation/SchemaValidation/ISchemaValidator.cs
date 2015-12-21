namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using Core.Scheme;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, string schemaNamespace, string schemaVersion);
    }
}
