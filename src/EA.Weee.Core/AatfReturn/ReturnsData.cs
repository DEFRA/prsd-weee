namespace EA.Weee.Core.AatfReturn
{
    using DataReturns;
    using Prsd.Core;
    using System;
    using System.Collections.Generic;

    public class ReturnsData
    {
        public List<ReturnData> ReturnsList { get; private set; }

        public Quarter ReturnQuarter { get; private set; }

        public List<Quarter> OpenQuarters { get; private set; }
        public QuarterWindow NextWindow { get; private set; }

        public DateTime CurrentDate { get; private set; }

        public ReturnsData(List<ReturnData> returnsList,
            Quarter returnQuarter,
            List<Quarter> openQuarters,
            QuarterWindow nextWindow,
            DateTime currentDate)
        {
            Guard.ArgumentNotNull(() => returnsList, returnsList);

            ReturnsList = returnsList;
            ReturnQuarter = returnQuarter;
            OpenQuarters = openQuarters;
            NextWindow = nextWindow;
            CurrentDate = currentDate;
        }
    }
}
