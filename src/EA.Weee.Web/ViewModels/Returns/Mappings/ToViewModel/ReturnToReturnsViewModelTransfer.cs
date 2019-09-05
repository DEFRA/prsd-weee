namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;

    public class ReturnToReturnsViewModelTransfer
    {
        public int? SelectedComplianceYear { get; set; }

        public string SelectedQuarter { get; set; }

        public ReturnsData ReturnsData { get; set; }
    }
}