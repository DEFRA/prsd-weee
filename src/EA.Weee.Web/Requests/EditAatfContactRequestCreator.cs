namespace EA.Weee.Web.Requests
{
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Aatf;

    public class EditAatfContactRequestCreator : IEditAatfContactRequestCreator
    {
        public EditAatfContact ViewModelToRequest(AatfEditContactAddressViewModel viewModel)
        {
            return new EditAatfContact(viewModel.AatfData.Id, viewModel.ContactData);
        }
    }
}