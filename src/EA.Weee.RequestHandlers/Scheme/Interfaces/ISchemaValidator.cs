namespace EA.Weee.RequestHandlers.Scheme.Interfaces
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using XmlValidation.Errors;

    public interface ISchemaValidator
    {
        IEnumerable<XmlValidationError> Validate(ProcessXMLFile message);
    }
}
