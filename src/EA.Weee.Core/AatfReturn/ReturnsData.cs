namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using DataReturns;
    using Prsd.Core;

    public class ReturnsData
    {
        public List<ReturnData> ReturnsList { get; private set; }

        public Quarter ReturnQuarter { get; private set; }

        public List<Quarter> OpenQuarters { get; private set; }
        public QuarterWindow NextWindow { get; private set; }

        public ReturnsData(List<ReturnData> returnsList,
            Quarter returnQuarter,
            List<Quarter> openQuarters,
            QuarterWindow nextWindow)
        {
            Guard.ArgumentNotNull(() => returnsList, returnsList);

            ReturnsList = returnsList;
            ReturnQuarter = returnQuarter;
            OpenQuarters = openQuarters;
            NextWindow = nextWindow;
        }
    }
}
