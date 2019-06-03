namespace EA.Weee.Web.ViewModels.Returns
{
    using Core.AatfReturn;

    public class ReturnsItemViewModel
    {
        public ReturnViewModel ReturnViewModel { get; set; }

        public ReturnsListDisplayOptions ReturnsListDisplayOptions { get; set; }

        public ReturnsListRedirectOptions ReturnsListRedirectOptions { get; set; }
    }
}