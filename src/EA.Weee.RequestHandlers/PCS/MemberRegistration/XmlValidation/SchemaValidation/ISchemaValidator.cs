namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation
{
    using System.Collections.Generic;
    using Domain.PCS;
    using Requests.PCS.MemberRegistration;

    public interface ISchemaValidator
    {
        IEnumerable<MemberUploadError> Validate(ProcessXMLFile message);
    }
}
