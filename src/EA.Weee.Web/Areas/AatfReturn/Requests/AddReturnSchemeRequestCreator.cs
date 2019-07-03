namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class AddReturnSchemeRequestCreator : RequestCreator<SelectYourPcsViewModel, AddReturnScheme>, IAddReturnSchemeRequestCreator
    {
        public override AddReturnScheme ViewModelToRequest(SelectYourPcsViewModel viewModel)
        {
            return new AddReturnScheme
            {
                ReturnId = viewModel.ReturnId,
                SchemeIds = viewModel.SelectedSchemes
            };
        }
    }
}