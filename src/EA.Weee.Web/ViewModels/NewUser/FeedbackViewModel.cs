namespace EA.Weee.Web.ViewModels.NewUser
{
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackViewModel : RadioButtonStringCollectionViewModel
    {
        [StringLength(1200)]
        public string FeedbackDescription { get; set; }
    }
}