namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using Prsd.Core;

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

        public virtual OrganisationData OrganisationData { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public IList<ReturnReportOn> ReturnReportOns { get; set; }

        public string CreatedBy { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public Core.AatfReturn.ReturnStatus ReturnStatus { get; set; }

        public Guid OrganisationId { get; set; }

        public FacilityType FacilityType { get; set; }

        public bool NilReturn { get; set; }

        public ReturnData()
        {
            ObligatedWeeeReceivedData = new List<WeeeObligatedData>();
            ObligatedWeeeReusedData = new List<WeeeObligatedData>();
            ObligatedWeeeSentOnData = new List<WeeeObligatedData>();
            NonObligatedData = new List<NonObligatedData>();
            SchemeDataItems = new List<SchemeData>();
            ReturnReportOns = new List<ReturnReportOn>();
        }

        public DateTime SystemDateTime { get; set; }
    }
}
