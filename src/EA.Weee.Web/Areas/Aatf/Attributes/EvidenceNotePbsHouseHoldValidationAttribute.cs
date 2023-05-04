namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Filters;
    using Services.Caching;

    [AttributeUsage(AttributeTargets.Property)]
    public class EvidenceNotePbsHouseHoldValidationAttribute : ValidationAttribute
    {
        public string RecipientProperty { get; set; }

        private IWeeeCache cache;

        public IWeeeCache Cache
        {
            get => cache ?? DependencyResolver.Current.GetService<IWeeeCache>();
            set => cache = value;
        }

        public EvidenceNotePbsHouseHoldValidationAttribute(string recipientProperty)
        {
            Condition.Requires(recipientProperty).IsNotNullOrEmpty();

            RecipientProperty = recipientProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var recipientValue = (Guid?)validationContext.ObjectType.GetProperty(RecipientProperty)?.GetValue(validationContext.ObjectInstance, null);

            if (recipientValue.HasValue)
            {
                var obligationValue = (WasteType?)value;

                if (obligationValue.HasValue)
                {
                    var organisationInfo = AsyncHelpers.RunSync(async () => await Cache.FetchSchemePublicInfo(recipientValue.Value));

                    if (organisationInfo.IsBalancingScheme && obligationValue != WasteType.Household)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}