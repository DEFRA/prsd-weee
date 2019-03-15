namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;

    public class ReturnAndSchemeDataToReceivedPcsViewModelMap : IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>> obligatedMap;
        private readonly ICategoryValueTotalCalculator calculator;

        public ReceivedPcsListViewModel Map(ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new ReceivedPcsListViewModel()
            {
                AatfId = source.AatfId,
                AatfName = source.AatfName,
                OrganisationId = source.OrganisationId,
                ReturnId = source.ReturnId
            };

            var schemeList = new List<ReceivedPcsData>();

            calculator.Total(source.ReturnData.ObligatedWeeeReceivedData)

            foreach (var scheme in source.SchemeDataItems)
            {
                var receivedPcsData = new ReceivedPcsData()
                {
                    ApprovalNumber = scheme.ApprovalName,
                    SchemeId = scheme.Id,
                    SchemeName = scheme.SchemeName
                };

                schemeList.Add(receivedPcsData);
            }

            viewModel.SchemeList = schemeList;

            return viewModel;
        }
    }
}