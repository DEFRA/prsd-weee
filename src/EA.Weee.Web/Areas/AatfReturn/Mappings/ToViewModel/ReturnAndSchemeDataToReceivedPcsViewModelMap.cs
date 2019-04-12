namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Services.Caching;

    public class ReturnAndSchemeDataToReceivedPcsViewModelMap : IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel>
    {
        private readonly IWeeeCache cache;
        private readonly ITonnageUtilities tonnageUtilities;

        public ReturnAndSchemeDataToReceivedPcsViewModelMap(
            IWeeeCache cache,
            ITonnageUtilities tonnageUtilities)
        {
            this.cache = cache;
            this.tonnageUtilities = tonnageUtilities;
        }

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

            foreach (var scheme in source.SchemeDataItems)
            {
                var weeeReceivedData = source.ReturnData.ObligatedWeeeReceivedData.Where(s => s.Scheme.Id == scheme.Id && s.Aatf.Id == source.AatfId).ToList();

                var receivedPcsData = new ReceivedPcsData()
                {
                    ApprovalNumber = scheme.ApprovalName,
                    SchemeId = scheme.Id,
                    SchemeName = scheme.SchemeName,
                    Tonnages = tonnageUtilities.SumObligatedValues(weeeReceivedData)
                };

                schemeList.Add(receivedPcsData);
            }

            viewModel.SchemeList = schemeList;

            return viewModel;
        }
    }
}