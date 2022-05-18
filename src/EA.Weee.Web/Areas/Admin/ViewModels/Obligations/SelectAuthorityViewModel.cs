namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SelectAuthorityViewModel
    {
        [Required(ErrorMessage = "Select an appropriate authority")]
        [Display(Name = "For which appropriate authority would you like to manage obligations?")]
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
    }
}