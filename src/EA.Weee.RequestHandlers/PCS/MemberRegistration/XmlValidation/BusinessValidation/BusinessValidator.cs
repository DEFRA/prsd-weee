namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.PCS;
    using FluentValidation;

    public class BusinessValidator : IBusinessValidator
    {
        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml)
        {
            var result = ((IValidator<schemeType>)(new SchemeTypeValidator())).Validate(deserializedXml);
            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, err.ErrorMessage));
        }
    }
}
