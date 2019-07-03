namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Core.Admin.AatfReports;
    using Core.Shared;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels.Aatf;
    using ITonnageUtilities = Web.ViewModels.Returns.Mappings.ToViewModel.ITonnageUtilities;

    public class AatfSubmissionHistoryDataToViewModelMap : IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;

        public AatfSubmissionHistoryDataToViewModelMap(ITonnageUtilities tonnageUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
        }

        public AatfSubmissionHistoryViewModel Map(AatfSubmissionHistoryData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new AatfSubmissionHistoryViewModel()
            {
                ReturnId = source.ReturnId,
                ComplianceYear = source.ComplianceYear, 
                Quarter = source.Quarter,
                SubmittedDate = source.SubmittedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                SubmittedBy = source.SubmittedBy,
                ObligatedTotal = tonnageUtilities.SumTotals(new List<decimal?>() { source.WeeeReceivedHouseHold, source.WeeeReusedHouseHold, source.WeeeSentOnHouseHold }),
                NonObligatedTotal = tonnageUtilities.SumTotals(new List<decimal?>() { source.WeeeReceivedNonHouseHold, source.WeeeReusedNonHouseHold, source.WeeeSentOnNonHouseHold })
            };
        }
    }
}