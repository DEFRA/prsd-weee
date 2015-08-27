namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Scheme;
    using FluentValidation;
    using FluentValidation.Internal;
    using Interfaces;
    using Queries;
    using Rules;

    public class BusinessValidator : IBusinessValidator
    {
        private readonly WeeeContext context;
        private readonly IRules rules;

        public static string RegistrationNoRuleSet = "registrationNo";

        public static string AuthorisedRepresentativeMustBeInUkRuleset = "authorisedRepresentativeMustBeInUk";

        public static string DataValidationRuleSet = "dataValidation";

        public BusinessValidator(WeeeContext context, IRules rules)
        {
            this.context = context;
            this.rules = rules;
        }

        public IEnumerable<MemberUploadError> Validate(schemeType deserializedXml, Guid pcsId)
        {
            var result = new SchemeTypeValidator(context, pcsId, rules).Validate(deserializedXml, new RulesetValidatorSelector("*"));
            return result.Errors.Select(err => new MemberUploadError((ErrorLevel)err.CustomState, MemberUploadErrorType.Business, err.ErrorMessage));
        }
    }
}
