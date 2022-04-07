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
                SelectedWeeeSentOnId = source.SelectedWeeeSentOnId
            };

            var siteList = new List<WeeeSearchedAnAatfListData>();

            foreach (var item in source.Sites)
            {
                var siteData = new WeeeSearchedAnAatfListData()
                {                    
                    SiteAddress = item.SiteAddress,
                    OperatorAddress = item.OperatorAddress,
                    ApprovalNumber = item.ApprovalNumber,
                    WeeeSentOnId = item.WeeeSentOnId
                };                
                siteList.Add(siteData);
            }

            viewModel.Sites = siteList;

            return viewModel;
        }
    }
}