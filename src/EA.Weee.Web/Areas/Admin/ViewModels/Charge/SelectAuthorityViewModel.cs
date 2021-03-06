﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using EA.Weee.Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SelectAuthorityViewModel
    {
        [Required(ErrorMessage = "Select an appropriate authority")]
        [Display(Name = "For which appropriate authority would you like to manage charges?")]
        public CompetentAuthority? SelectedAuthority { get; set; }

        public IReadOnlyCollection<CompetentAuthority> PossibleValues { get; private set; }

        public SelectAuthorityViewModel()
        {
            PossibleValues = new List<CompetentAuthority>()
            {
                CompetentAuthority.England,
                CompetentAuthority.Scotland,
                CompetentAuthority.NorthernIreland,
                CompetentAuthority.Wales
            };
        }
    }
}