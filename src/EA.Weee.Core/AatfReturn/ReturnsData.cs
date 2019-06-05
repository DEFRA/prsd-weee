namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;
    using Prsd.Core;

    public class ReturnsData
    {
        public IList<ReturnData> ReturnsList { get; private set; }

        public ReturnQuarter ReturnQuarter { get; private set; }

        public ReturnsData(IList<ReturnData> returnsList,
            ReturnQuarter returnQuarter)
        {
            Guard.ArgumentNotNull(() => returnsList, returnsList);

            ReturnsList = returnsList;
            ReturnQuarter = returnQuarter;
        }
    }
}
