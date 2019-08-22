namespace EA.Weee.XmlValidation.SchemaValidation
{
    using Errors;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface INamespaceValidator
    {
        IEnumerable<XmlValidationError> Validate(XNamespace expectedNamespace, XNamespace actualNamespace);
    }
}
