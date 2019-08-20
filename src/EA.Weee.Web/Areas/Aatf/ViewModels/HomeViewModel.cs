namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class HomeViewModel : IValidatableObject
    {
        public Guid OrganisationId { get; set; }

        public bool IsAE { get; set; }

        [DisplayName("Which organisation would you like to perform activities for?")]
        public Guid? SelectedAatfId { get; set; }

        [DisplayName("Which organisation would you like to perform activities for?")]
        public Guid? SelectedAeId { get; set; }

        public IReadOnlyList<AatfData> AatfList { get; set; }

        public bool ModelValidated { get; private set; }

        public HomeViewModel()
        {
        }

        public static IEnumerable<string> ValidationMessageDisplayOrder => new List<string>
        {
            nameof(SelectedAatfId),
            nameof(SelectedAeId)
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var instance = validationContext.ObjectInstance as HomeViewModel;

            if (instance != null)
            {
                if (instance.IsAE && !instance.SelectedAeId.HasValue)
                {
                    validationResults.Add(
                        new ValidationResult($"Select an AE to perform activities"));
                }
                else if (!instance.IsAE && !instance.SelectedAatfId.HasValue)
                {
                    validationResults.Add(
                        new ValidationResult($"Select an AATF to perform activities"));
                }
            }

            ModelValidated = true;
            return validationResults;
        }
    }
}