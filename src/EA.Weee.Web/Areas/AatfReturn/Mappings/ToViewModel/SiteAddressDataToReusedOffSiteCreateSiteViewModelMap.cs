﻿namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System.Linq;

    public class SiteAddressDataToReusedOffSiteCreateSiteViewModelMap : IMap<SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer, ReusedOffSiteCreateSiteViewModel>
    {
        public ReusedOffSiteCreateSiteViewModel Map(SiteAddressDataToReusedOffSiteCreateSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.Countries, source.Countries);

            var viewModel = new ReusedOffSiteCreateSiteViewModel
            {
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId,
                SiteId = source.SiteId,
                AddressData = new SiteAddressData(),
                HasSites = source.ReturnedSites.AddressData.Count() > 0 ? true : false
            };

            viewModel.AddressData.Countries = source.Countries;

            if (source.SiteId.HasValue)
            {
                viewModel.AddressData = source.ReturnedSites.AddressData.FirstOrDefault(s => s.Id == source.SiteId);
                if (viewModel.AddressData != null)
                {
                    viewModel.AddressData.Countries = source.Countries;
                }
            }

            return viewModel;
        }
    }
}