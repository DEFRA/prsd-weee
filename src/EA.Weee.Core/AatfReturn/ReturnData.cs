namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReturnData
    {
        public Guid Id { get; set; }

        public Quarter Quarter { get; set; }

        public QuarterWindow QuarterWindow { get; set; }
    }
}
