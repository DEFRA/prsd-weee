namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Admin.AatfReports;
    using Core.DataReturns;
    using Core.Shared;
    using Domain.Admin.AatfReports;
    using Prsd.Core.Mapper;

    public class AatfSubmissionHistoryMap : IMap<AatfSubmissionHistory, AatfSubmissionHistoryData>
    {
        public AatfSubmissionHistoryData Map(AatfSubmissionHistory source)
        {
            return new AatfSubmissionHistoryData()
            {
                ReturnId = source.ReturnId,
                Quarter = (QuarterType)source.Quarter,
                ComplianceYear = source.ComplianceYear,
                WeeeReceivedHouseHold = source.WeeeReceivedHouseHold,
                WeeeReceivedNonHouseHold = source.WeeeReceivedNonHouseHold,
                WeeeReusedHouseHold = source.WeeeReusedHouseHold,
                WeeeReusedNonHouseHold = source.WeeeReusedHouseNonHold,
                WeeeSentOnHouseHold = source.WeeeSentOnHouseHold,
                WeeeSentOnNonHouseHold = source.WeeeSentOnNonHouseHold,
                SubmittedDate = source.SubmittedDate,
                SubmittedBy = source.SubmittedBy
            };
        }
    }
}
