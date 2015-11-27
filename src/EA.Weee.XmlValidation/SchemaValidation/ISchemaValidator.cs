namespace EA.Weee.XmlValidation.SchemaValidation
{
    using Core.Scheme;
    using System.Collections.Generic;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, string schemaNamespace, SchemaVersion schemaVersion);
    }
}
