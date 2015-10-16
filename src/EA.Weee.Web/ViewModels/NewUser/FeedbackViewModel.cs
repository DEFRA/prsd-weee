namespace EA.Weee.Web.ViewModels.NewUser
{
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class FeedbackViewModel : RadioButtonStringCollectionViewModel
    {
        [StringLength(1200)]
        public string FeedbackDescription { get; set; }
    }
}