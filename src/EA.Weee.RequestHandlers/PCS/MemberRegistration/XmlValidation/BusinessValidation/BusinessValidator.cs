namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.PCS;
    using FluentValidation;
    using FluentValidation.Internal;

    public class BusinessValidator : IBusinessValidator
    {
        public static string RegistrationNoRuleSet = "registrationNo";

        public static string AuthorisedRepresentativeMustBeInUkRuleset = "authorisedRepresentativeMustBeInUk";

        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml)
        {
            var result = new SchemeTypeValidator().Validate(deserializedXml, new RulesetValidatorSelector("*"));
            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, err.ErrorMessage));
        }
    }
}
