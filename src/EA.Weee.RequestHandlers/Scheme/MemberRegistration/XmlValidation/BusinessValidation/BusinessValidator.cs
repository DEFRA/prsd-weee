namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.XmlBusinessValidation;
    using DataAccess;
    using Domain;
    using Domain.Scheme;
    using FluentValidation;
    using FluentValidation.Internal;
    using Interfaces;

    public class BusinessValidator : IBusinessValidator
    {
        private readonly WeeeContext context;
        private readonly IRuleSelector ruleSelector;

        public static string RegistrationNoRuleSet = "registrationNo";

        public static string AuthorisedRepresentativeMustBeInUkRuleset = "authorisedRepresentativeMustBeInUk";

        public static string DataValidationRuleSet = "dataValidation";

        public static string CustomRules = "customrules";

        public BusinessValidator(WeeeContext context, IRuleSelector ruleSelector)
        {
            this.context = context;
            this.ruleSelector = ruleSelector;
        }

        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml, Guid pcsId)
        {
            var result = new SchemeTypeValidator(context, pcsId, ruleSelector).Validate(deserializedXml, new RulesetValidatorSelector("*"));
            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, MemberUploadErrorType.Business, err.ErrorMessage));
        }
    }
}
