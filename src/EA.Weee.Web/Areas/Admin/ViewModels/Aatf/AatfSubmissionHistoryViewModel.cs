namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using Core.DataReturns;

    public class AatfSubmissionHistoryViewModel
    {
        public virtual Guid ReturnId { get; set; }

        public QuarterType Quarter { get; set; }

        public int ComplianceYear { get; set; }

        public string SubmittedBy { get; set; }

        public string SubmittedDate { get; set; }

        public string ObligatedTotal { get; set; }

        public string NonObligatedTotal { get; set; }

        //public string ObligatedTotal
        //{
        //    get
        //    {
        //        decimal? total = null;

        //        if (WeeeReceivedHouseHold.HasValue)
        //        {
        //            total = 0 + WeeeReceivedHouseHold.Value;
        //        }

        //        if (WeeeReusedHouseHold.HasValue)
        //        {
        //            total = InitialiseTotal(total);
        //            if (WeeeReusedHouseHold != null)
        //            {
        //                total += WeeeReusedHouseHold.Value;
        //            }
        //        }

        //        if (WeeeSentOnHouseHold.HasValue)
        //        {
        //            total = InitialiseTotal(total);
        //            if (WeeeSentOnHouseHold != null)
        //            {
        //                total += WeeeSentOnHouseHold.Value;
        //            }
        //        }

        //        return tonnageUtilities.CheckIfTonnageIsNull(total);
        //    }
        //}

        //public string NonObligatedTotal
        //{
        //    get
        //    {
        //        decimal? total = null;

        //        if (WeeeReceivedNonHouseHold.HasValue)
        //        {
        //            total = 0 + WeeeReceivedNonHouseHold.Value;
        //        }

        //        if (WeeeReusedNonHouseHold.HasValue)
        //        {
        //            total = InitialiseTotal(total);
        //            if (WeeeReusedNonHouseHold != null)
        //            {
        //                total += WeeeReusedNonHouseHold.Value;
        //            }
        //        }

        //        if (WeeeSentOnNonHouseHold.HasValue)
        //        {
        //            total = InitialiseTotal(total);
        //            if (WeeeSentOnNonHouseHold != null)
        //            {
        //                total += WeeeSentOnNonHouseHold.Value;
        //            }
        //        }

        //        return tonnageUtilities.CheckIfTonnageIsNull(total);
        //    }
        //}
    }
}