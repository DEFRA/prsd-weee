namespace EA.Weee.Web.Areas.Admin.ViewModels.User
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.Shared;

    public class FilteringViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Organisation name")]
        public string OrganisationName { get; set; }

        public UserStatus Status { get; set; }
    }
}