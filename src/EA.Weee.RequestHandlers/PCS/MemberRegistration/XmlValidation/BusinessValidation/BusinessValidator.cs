namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using FluentValidation;
    using FluentValidation.Internal;

    public class BusinessValidator : IBusinessValidator
    {
        private readonly WeeeContext context;

        public static string RegistrationNoRuleSet = "registrationNo";

        public static string AuthorisedRepresentativeMustBeInUkRuleset = "authorisedRepresentativeMustBeInUk";

        public static string DataValidationRuleSet = "dataValidation";

        public BusinessValidator(WeeeContext context)
        {
            this.context = context;
        }

        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml, Guid pcsId)
        {
            var result = new SchemeTypeValidator(context, pcsId).Validate(deserializedXml, new RulesetValidatorSelector("*"));
            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, MemberUploadErrorType.Business, err.ErrorMessage));
        }
    }
}
