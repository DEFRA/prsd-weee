namespace EA.Weee.Web.Areas.AatfReturn.Requests
{
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System;

    public class EditSentOnAatfSiteRequestCreator : IEditSentOnAatfSiteRequestCreator
    {
        public SentOnAatfSite ViewModelToRequest(SentOnCreateSiteOperatorViewModel viewModel)
        {
            if (viewModel.OperatorAddressFound)
            {
                return new EditSentOnAatfSite()
                {
                    OperatorAddressData = viewModel.OperatorAddressData,
                    OperatorAddressId = (Guid)viewModel.OperatorAddressId,
                    WeeeSentOnId = (Guid)viewModel.WeeeSentOnId
                };
            }

            var aatfSite = new EditSentOnAatfSiteWithOperator()
            {
                OrganisationId = viewModel.OrganisationId,
                ReturnId = viewModel.ReturnId,
                AatfId = viewModel.AatfId,
                SiteAddressData = viewModel.SiteAddressData,
                WeeeSentOnId = viewModel.WeeeSentOnId,
                OperatorAddressData = viewModel.OperatorAddressData
            };

            return aatfSite;
        }
    }
}
