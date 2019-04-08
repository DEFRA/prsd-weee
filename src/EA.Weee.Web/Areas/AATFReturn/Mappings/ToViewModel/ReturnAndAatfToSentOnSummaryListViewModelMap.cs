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
    
    public class ReturnAndAatfToSentOnSummaryListViewModelMap : IMap<ReturnAndAatfToSentOnSummaryListViewModelMapTransfer, SentOnSiteSummaryListViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly ITonnageUtilities tonnageUtilities;

        public ReturnAndAatfToSentOnSummaryListViewModelMap(
            IWeeeCache cache,
            ITonnageUtilities tonnageUtilities)
        {
            this.cache = cache;
            this.tonnageUtilities = tonnageUtilities;
        }
        
        public SentOnSiteSummaryListViewModel Map(ReturnAndAatfToSentOnSummaryListViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new SentOnSiteSummaryListViewModel()
            {
                AatfId = source.AatfId,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId
            };

            var schemeList = new List<WeeeSentOnSummaryListData>();

            foreach (var item in source.WeeeSentOnDataItems)
            {
                var receivedPcsData = new WeeeSentOnSummaryListData()
                {
                    Tonnages = tonnageUtilities.SumObligatedValues(item.Tonnages),
                    SiteAddress = item.SiteAddress,
                    OperatorAddress = item.OperatorAddress
                };

                schemeList.Add(receivedPcsData);
            }

            viewModel.Sites = schemeList;

            return viewModel;
        }
    }
}