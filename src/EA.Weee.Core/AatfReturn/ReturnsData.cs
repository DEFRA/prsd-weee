namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using DataReturns;
    using Prsd.Core;

    public class ReturnsData
    {
        public List<ReturnData> ReturnsList { get; private set; }

        public Quarter ReturnQuarter { get; private set; }

        public ReturnsData(List<ReturnData> returnsList,
            Quarter returnQuarter)
        {
            Guard.ArgumentNotNull(() => returnsList, returnsList);

            ReturnsList = returnsList;
            ReturnQuarter = returnQuarter;
        }
    }
}
