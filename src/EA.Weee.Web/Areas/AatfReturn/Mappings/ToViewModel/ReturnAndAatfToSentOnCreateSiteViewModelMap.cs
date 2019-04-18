namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ReturnAndAatfToSentOnCreateSiteViewModelMap : IMap<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer, SentOnCreateSiteViewModel>
    {
        private readonly IWeeeCache cache;

        public ReturnAndAatfToSentOnCreateSiteViewModelMap(IWeeeCache cache)
        {
            this.cache = cache;
        }

        public SentOnCreateSiteViewModel Map(ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            if (source.SiteAddressData.Name == null)
            {
                source.SiteAddressData = new AatfAddressData();
            }

            source.SiteAddressData.Countries = source.CountryData;

            var viewModel = new SentOnCreateSiteViewModel()
            {
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                SiteAddressData = source.SiteAddressData
            };

            return viewModel;
        }
    }
}