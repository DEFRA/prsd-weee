namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Domain;
    using FluentValidation;
    using Requests.PCS.MemberRegistration;

    public interface IXmlValidator
    {
        IEnumerable<MemberUploadError> Validate(ValidateXmlFile message);
    }
}
