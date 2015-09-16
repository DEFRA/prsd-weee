namespace EA.Weee.Web.ViewModels.NewUser
{
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class FeedbackViewModel
    {
        [Required]
        public RadioButtonStringCollectionViewModel FeedbackOptions { get; set; }

        [StringLength(1200)]
        public string FeedbackDescription { get; set; }
    }
}