namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WeeeSentOnSummaryListData
    {
        public ObligatedCategoryValue Tonnages { get; set; }

        public virtual AatfAddressData OperatorAddress { get; set; }

        public virtual AatfAddressData SiteAddress { get; set; }
    }
}
