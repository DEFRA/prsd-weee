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
        public SentOnCreateSiteViewModel Map(ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SentOnCreateSiteViewModel()
            {
                ReturnId = source.Return.Id,
                AatfId = source.AatfId,
                OrganisationId = source.Return.OrganisationData.Id,
            };

            if (source.WeeeSentOnData != null)
            {
                viewModel.OperatorAddressData =
                    new OperatorAddressData(source.WeeeSentOnData.OperatorAddress.Name,
                        source.WeeeSentOnData.OperatorAddress.Address1,
                        source.WeeeSentOnData.OperatorAddress.Address2,
                        source.WeeeSentOnData.OperatorAddress.TownOrCity,
                        source.WeeeSentOnData.OperatorAddress.CountyOrRegion,
                        source.WeeeSentOnData.OperatorAddress.Postcode,
                        source.WeeeSentOnData.OperatorAddress.CountryId,
                        source.WeeeSentOnData.OperatorAddress.CountryName)
                    {
                        Id = source.WeeeSentOnData.OperatorAddressId
                    };

                viewModel.SiteAddressData = new AatfAddressData(source.WeeeSentOnData.SiteAddressId,
                    source.WeeeSentOnData.SiteAddress.Name,
                    source.WeeeSentOnData.SiteAddress.Address1,
                    source.WeeeSentOnData.SiteAddress.Address2,
                    source.WeeeSentOnData.SiteAddress.TownOrCity,
                    source.WeeeSentOnData.SiteAddress.CountyOrRegion,
                    source.WeeeSentOnData.SiteAddress.Postcode,
                    source.WeeeSentOnData.SiteAddress.CountryId,
                    source.WeeeSentOnData.SiteAddress.CountryName);
            }
            else
            {
                viewModel.SiteAddressData = new AatfAddressData();
                viewModel.OperatorAddressData = new OperatorAddressData();
            }

            viewModel.SiteAddressData.Countries = source.CountryData;
            viewModel.OperatorAddressData.Countries = source.CountryData;

            return viewModel;
        }
    }
}