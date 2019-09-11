namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.Helpers;

    public class HomeViewModel : IValidatableObject
    {
        public Guid OrganisationId { get; set; }

        public FacilityType FacilityType { get; set; }

        [DisplayName("Which organisation would you like to perform activities for?")]
        public Guid? SelectedId { get; set; }

        public IReadOnlyList<AatfData> AatfList { get; set; }

        public bool ModelValidated { get; private set; }

        public HomeViewModel()
        {
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var instance = validationContext.ObjectInstance as HomeViewModel;

            if (instance != null)
            {
                if (!instance.SelectedId.HasValue)
                {
                    validationResults.Add(
                        new ValidationResult($"Select an {FacilityType.ToDisplayString()} to perform activities"));
                }
            }

            ModelValidated = true;
            return validationResults;
        }
    }
}