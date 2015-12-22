namespace EA.Weee.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Errors;

    public interface INamespaceValidator
    {
        IEnumerable<XmlValidationError> Validate(string expectedNamespace, string actualNamespace);
    }
}
