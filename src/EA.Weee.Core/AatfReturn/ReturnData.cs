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

        public virtual OperatorData ReturnOperatorData { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public ReturnData()
        {
            ObligatedWeeeReceivedData = new List<WeeeObligatedData>();
            ObligatedWeeeReusedData = new List<WeeeObligatedData>();
            NonObligatedData = new List<NonObligatedData>();
            SchemeDataItems = new List<SchemeData>();
        }
    }
}
