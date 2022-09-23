namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System.Collections.Generic;

    public class ReturnAndAatfToSearchedAatfViewModelMap : IMap<ReturnAndAatfToSearchedAatfViewModelMapTransfer, SearchedAatfResultListViewModel>
    {
        public SearchedAatfResultListViewModel Map(ReturnAndAatfToSearchedAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SearchedAatfResultListViewModel()
            {
                ReturnId = source.ReturnId,
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                AatfName = source.AatfName,
                SelectedAatfId = source.SelectedAatfId,
                SelectedAatfName = source.SelectedAatfName,
                SelectedSiteName = source.SelectedSiteName
            };

            var siteList = new List<WeeeSearchedAnAatfListData>();

            foreach (var item in source.AatfDataList)
            {
                siteList.Add(new WeeeSearchedAnAatfListData()
                {
                    SiteAddress = item.SiteAddress,
                    OperatorAddress = new AatfAddressData()
                    {
                        Address1 = item.Organisation.BusinessAddress.Address1,
                        Address2 = item.Organisation.BusinessAddress.Address2,
                        Countries = item.Organisation.BusinessAddress.Countries,
                        CountryName = item.Organisation.BusinessAddress.CountryName,
                        TownOrCity = item.Organisation.BusinessAddress.TownOrCity,
                        Postcode = item.Organisation.BusinessAddress.Postcode,
                        CountyOrRegion = item.Organisation.BusinessAddress.CountyOrRegion,
                        Name = item.Organisation.Name,
                        CountryId = item.Organisation.BusinessAddress.CountryId,
                        Id = item.Organisation.BusinessAddress.Id
                    },
                    ApprovalNumber = item.ApprovalNumber,
                    WeeeAatfId = item.Id
                });
            }

            viewModel.Sites = siteList;

            return viewModel;
        }
    }
}