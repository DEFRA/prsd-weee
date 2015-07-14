namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Domain;
    using Requests.PCS.MemberRegistration;

    public interface ISchemaValidator
    {
        IEnumerable<MemberUploadError> Validate(ValidateXmlFile message);
    }
}
