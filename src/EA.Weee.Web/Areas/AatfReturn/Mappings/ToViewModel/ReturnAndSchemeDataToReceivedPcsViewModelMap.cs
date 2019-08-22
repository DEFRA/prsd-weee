﻿namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using ITonnageUtilities = Web.ViewModels.Returns.Mappings.ToViewModel.ITonnageUtilities;

    public class ReturnAndSchemeDataToReceivedPcsViewModelMap : IMap<ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer, ReceivedPcsListViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;

        public ReturnAndSchemeDataToReceivedPcsViewModelMap(
            ITonnageUtilities tonnageUtilities)
        {
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