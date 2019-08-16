namespace EA.Weee.Web.Areas.Admin.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
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