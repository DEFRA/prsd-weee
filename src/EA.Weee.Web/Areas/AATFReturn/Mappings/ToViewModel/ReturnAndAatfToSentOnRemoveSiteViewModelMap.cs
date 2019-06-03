namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMap : IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;

        public ReturnAndAatfToSentOnRemoveSiteViewModelMap(ITonnageUtilities tonnageUtilities, 
            IAddressUtilities addressUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
            this.addressUtilities = addressUtilities;
        }

        public SentOnRemoveSiteViewModel Map(ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var tonnages = tonnageUtilities.SumObligatedValues(source.WeeeSentOn.Tonnages);

            var viewModel = new SentOnRemoveSiteViewModel()
            {
                WeeeSentOn = source.WeeeSentOn,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                SiteAddress = addressUtilities.FormattedAddress(source.WeeeSentOn.SiteAddress),
                OperatorAddress = addressUtilities.FormattedAddress(source.WeeeSentOn.OperatorAddress),
                TonnageB2B = tonnages.B2B,
                TonnageB2C = tonnages.B2C
            };

            return viewModel;
        }
    }
}