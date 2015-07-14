namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Domain;
    using Requests.PCS.MemberRegistration;

    public interface IBusinessValidator
    {
        IEnumerable<MemberUploadError> Validate(ValidateXmlFile message);
    }
}
