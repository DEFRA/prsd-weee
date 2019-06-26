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

            var insertOperatorAddress = new OperatorAddressData();

            if (source.SiteAddressData == null)
            {
                source.SiteAddressData = new AatfAddressData();
            }

            if (source.OperatorAddressData != null)
            {
                insertOperatorAddress.Name = source.OperatorAddressData.Name;
                insertOperatorAddress.Address1 = source.OperatorAddressData.Address1;
                insertOperatorAddress.Address2 = source.OperatorAddressData.Address2;
                insertOperatorAddress.TownOrCity = source.OperatorAddressData.TownOrCity;
                insertOperatorAddress.CountyOrRegion = source.OperatorAddressData.CountyOrRegion;
                insertOperatorAddress.Postcode = source.OperatorAddressData.Postcode;
                insertOperatorAddress.CountryId = source.OperatorAddressData.CountryId;
                insertOperatorAddress.CountryName = source.OperatorAddressData.CountryName;
            }

            source.SiteAddressData.Countries = source.CountryData;
            insertOperatorAddress.Countries = source.CountryData;

            var viewModel = new SentOnCreateSiteViewModel()
            {
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                SiteAddressData = source.SiteAddressData,
                SiteAddressId = source.SiteAddressId,
                OperatorAddressData = insertOperatorAddress,
                OperatorAddressId = source.OperatorAddressId
            };

            if (source.JavascriptDisabled == true)
            {
                viewModel.OperatorAddressData.Address1 = source.SiteAddressData.Address1;
                viewModel.OperatorAddressData.Address2 = source.SiteAddressData.Address2;
                viewModel.OperatorAddressData.CountryId = source.SiteAddressData.CountryId;
                viewModel.OperatorAddressData.CountryName = source.SiteAddressData.CountryName;
                viewModel.OperatorAddressData.TownOrCity = source.SiteAddressData.TownOrCity;
                viewModel.OperatorAddressData.Postcode = source.SiteAddressData.Postcode;
                viewModel.OperatorAddressData.CountyOrRegion = source.SiteAddressData.CountyOrRegion;
            }

            return viewModel;
        }
    }
}