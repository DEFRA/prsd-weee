namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;

    public class ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap : IMap<ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer, SentOnCreateSiteOperatorViewModel>
    {
        private readonly IWeeeCache cache;

        public ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap(IWeeeCache cache)
        {
            this.cache = cache;
        }

        public SentOnCreateSiteOperatorViewModel Map(ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SentOnCreateSiteOperatorViewModel()
            {
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                OperatorAddressData = source.OperatorAddressData
            };

            return viewModel;
        }
    }
}