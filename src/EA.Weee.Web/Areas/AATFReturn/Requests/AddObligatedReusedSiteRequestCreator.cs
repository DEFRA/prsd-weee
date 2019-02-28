namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddObligatedReusedSiteRequestCreator : IAddObligatedReusedSiteRequestCreator
    {
        public override AddAatfSite ViewModelToRequest(ReusedOffSiteCreateSiteViewModel viewModel)
        {
            var addAatfSite = new AddAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                WeeeReusedId = viewModel.
            };
        }
    }
}