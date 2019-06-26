namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System;

    public class AddSentOnAatfSiteRequestCreator : IAddSentOnAatfSiteRequestCreator
    {
        public SentOnAatfSite ViewModelToRequest(SentOnCreateSiteViewModel viewModel)
        {
            if (viewModel.Edit)
            {
                return new EditSentOnAatfSite()
                {
                    SiteAddressData = viewModel.SiteAddressData,
                    SiteAddressId = (Guid)viewModel.SiteAddressId,
                    WeeeSentOnId = (Guid)viewModel.WeeeSentOnId,
                    OperatorAddressData = viewModel.OperatorAddressData,
                    OperatorAddressId = (Guid)viewModel.OperatorAddressId
                };
            }

            var aatfSite = new AddSentOnAatfSite()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                SiteAddressData = viewModel.SiteAddressData,
                OperatorAddressData = viewModel.OperatorAddressData
            };

            return aatfSite;
        }
    }
}