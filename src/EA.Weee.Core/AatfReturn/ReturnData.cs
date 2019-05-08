namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;

    public class ReturnData
    {
        public Guid Id { get; set; }

        public Quarter Quarter { get; set; }

        public QuarterWindow QuarterWindow { get; set; }

        public List<AatfData> Aatfs { get; set; }

        public List<NonObligatedData> NonObligatedData { get; set; }

        public List<WeeeObligatedData> ObligatedWeeeReceivedData { get; set; }

        public List<WeeeObligatedData> ObligatedWeeeReusedData { get; set; }

        public List<WeeeObligatedData> ObligatedWeeeSentOnData { get; set; }

        public virtual OperatorData ReturnOperatorData { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public IList<ReturnReportOn> ReturnReportOns { get; set; }

        public string CreatedBy { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public ReturnData()
        {
            ObligatedWeeeReceivedData = new List<WeeeObligatedData>();
            ObligatedWeeeReusedData = new List<WeeeObligatedData>();
            ObligatedWeeeSentOnData = new List<WeeeObligatedData>();
            NonObligatedData = new List<NonObligatedData>();
            SchemeDataItems = new List<SchemeData>();
            ReturnReportOns = new List<ReturnReportOn>();
        }
    }
}
