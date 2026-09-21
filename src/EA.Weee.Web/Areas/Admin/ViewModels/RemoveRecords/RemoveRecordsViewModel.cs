namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class RemoveRecordsViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "Select the activity you would like to do")]
        public override string SelectedValue { get; set; }
    }
}