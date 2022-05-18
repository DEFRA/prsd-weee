namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SelectAuthorityViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Select an appropriate authority")]
        [Display(Name = "Which appropriate authority are you managing obligations for?")]
        public CompetentAuthority? SelectedAuthority { get; set; }

        public IReadOnlyCollection<CompetentAuthority> PossibleValues { get; private set; }

        public SelectAuthorityViewModel()
        {
            PossibleValues = new List<CompetentAuthority>()
            {
                CompetentAuthority.England,
                CompetentAuthority.Scotland,
                CompetentAuthority.NorthernIreland
                //CompetentAuthority.Wales -- will be enabled in later dev -- check cshtml options
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.SelectedAuthority == null || !this.SelectedAuthority.HasValue)
            {
                yield return new ValidationResult("You must select an option ", new[] { "SelectedAuthority" });
            }
        }
    }
}