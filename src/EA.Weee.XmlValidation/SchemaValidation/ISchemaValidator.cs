namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(byte[] data);
    }
}
