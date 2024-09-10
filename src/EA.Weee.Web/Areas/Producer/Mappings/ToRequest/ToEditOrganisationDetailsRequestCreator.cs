namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;
    using System;

    public class ToEditOrganisationDetailsRequestCreator : IRequestCreator<EditOrganisationDetailsViewModel, EditProducerSubmissionAddressRequest>
    {
        public EditProducerSubmissionAddressRequest ViewModelToRequest(EditOrganisationDetailsViewModel viewModel)
        {
            return new EditProducerSubmissionAddressRequest();
        }
    }
}