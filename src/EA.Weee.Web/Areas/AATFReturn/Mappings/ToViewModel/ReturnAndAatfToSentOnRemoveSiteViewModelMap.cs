namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMap : IMap<ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer, SentOnRemoveSiteViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly ITonnageUtilities tonnageUtilities;

        public ReturnAndAatfToSentOnRemoveSiteViewModelMap(
            IWeeeCache cache,
            ITonnageUtilities tonnageUtilities)
        {
            this.cache = cache;
            this.tonnageUtilities = tonnageUtilities;
        }

        public SentOnRemoveSiteViewModel Map(ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var tonnages = tonnageUtilities.SumObligatedValues(source.WeeeSentOn.Tonnages);

            if (tonnages.B2C == "-")
            {
                tonnages.B2C = "0.000";
            }

            if (tonnages.B2B == "-")
            {
                tonnages.B2B = "0.000";
            }

            var viewModel = new SentOnRemoveSiteViewModel()
            {
                WeeeSentOn = source.WeeeSentOn,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                SiteAddress = source.SiteAddress,
                OperatorAddress = source.OperatorAddress,
                TonnageB2B = decimal.Parse(tonnages.B2B),
                TonnageB2C = decimal.Parse(tonnages.B2C)
            };

            return viewModel;
        }
    }
}