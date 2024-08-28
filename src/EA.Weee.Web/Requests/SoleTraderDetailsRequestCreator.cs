namespace EA.Weee.Web.Requests
{
    using EA.Weee.Core.Organisations;
    using Requests.Base;
    using Weee.Requests.Organisations.Create;

    public class SoleTraderDetailsRequestCreator : RequestCreator<SoleTraderDetailsViewModel, CreateSoleTraderRequest>, ISoleTraderDetailsRequestCreator
    {
        public override CreateSoleTraderRequest ViewModelToRequest(SoleTraderDetailsViewModel viewModel)
        {
            // Auto mappings
            var request = base.ViewModelToRequest(viewModel);

            // Manual mappings (where names differ)
            request.TradingName = viewModel.BusinessTradingName;

            return request;
        }
    }
}