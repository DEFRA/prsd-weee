namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;

    public class AatfObligatedData
    {
        public AatfObligatedData(Guid aatfId, string aatfName, List<ProcessObligatedData> processWeeeData)
        {
            AatfId = aatfId;
            AatfName = aatfName;
            ProcessObligatedData = processWeeeData;
        }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }
        
        public List<ProcessObligatedData> ProcessObligatedData { get; set; }
    }
}
