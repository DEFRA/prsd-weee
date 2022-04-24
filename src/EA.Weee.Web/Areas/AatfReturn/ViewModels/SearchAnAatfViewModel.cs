namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SearchAnAatfViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid? WeeeSentOnId { get; set; }

        public Guid SelectedAatfId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Search Term must have min length of 3 and max Length of 50")]
        [Display(Name = "Search Term")]
        public string SearchTerm { get; set; }
    }
}