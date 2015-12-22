namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Core.Scheme;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(byte[] data, string schemaName, XNamespace schemaNamespace, string schemaVersion);
    }
}
