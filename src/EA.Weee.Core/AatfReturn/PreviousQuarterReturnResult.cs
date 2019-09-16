namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataReturns;
    using System;
    using System.Collections.Generic;

    public class PreviousQuarterReturnResult
    {
        public List<Guid> PreviousSchemes { get; set; }

        public Quarter PreviousQuarter { get; set; }

        public PreviousQuarterReturnResult()
        {
            this.PreviousSchemes = new List<Guid>();
        }
    }
}
