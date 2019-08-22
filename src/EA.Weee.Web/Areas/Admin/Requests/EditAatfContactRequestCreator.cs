namespace EA.Weee.Web.Areas.Admin.Requests
{
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using Weee.Requests.Admin.Aatf;

    public class EditAatfContactRequestCreator : IEditAatfContactRequestCreator
    {
        public EditAatfContact ViewModelToRequest(AatfEditContactAddressViewModel viewModel)
        {
            return new EditAatfContact() { ContactData = viewModel.ContactData };
        }
    }
}