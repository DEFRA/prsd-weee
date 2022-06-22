namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Core.AatfReturn;

    public class SelectYourAatfViewModel : IValidatableObject
    {
        public Guid OrganisationId { get; set; }

        [DisplayName("Which site would you like to manage evidence notes for?")]
        [Required(ErrorMessage = "Select the site you would like to manage evidence notes for")]
        public Guid? SelectedId { get; set; }

        public IReadOnlyList<AatfData> AatfList { get; set; }

        public bool ModelValidated { get; private set; }

        public SelectYourAatfViewModel() 
        { 
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (validationContext.ObjectInstance is SelectYourAatfViewModel instance)
            {
                if (!instance.SelectedId.HasValue)
                {
                    validationResults.Add(
                        new ValidationResult($"Select the site you would like to manage evidence notes for"));
                }
            }

            ModelValidated = true;
            return validationResults;
        }
    }
}