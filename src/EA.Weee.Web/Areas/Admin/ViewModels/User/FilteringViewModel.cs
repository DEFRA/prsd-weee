namespace EA.Weee.Web.Areas.Admin.ViewModels.User
{
    using EA.Weee.Core.Shared;
    using System.ComponentModel.DataAnnotations;

    public class FilteringViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Organisation name")]
        public string OrganisationName { get; set; }

        public UserStatus? Status { get; set; }
    }
}