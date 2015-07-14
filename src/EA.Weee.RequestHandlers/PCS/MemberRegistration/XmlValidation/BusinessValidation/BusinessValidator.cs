namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Domain;
    using Domain.PCS;
    using FluentValidation;
    using Requests.PCS.MemberRegistration;

    public class BusinessValidator : IBusinessValidator
    {
        public IEnumerable<MemberUploadError> Validate(ValidateXmlFile message)
        {
            var doc = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
            var deserialzedXml = new XmlSerializer(typeof(schemeType)).Deserialize(doc.CreateReader());

            var result = ((IValidator<schemeType>)(new SchemeTypeValidator())).Validate(deserialzedXml);

            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, err.ErrorMessage));
        }
    }
}
