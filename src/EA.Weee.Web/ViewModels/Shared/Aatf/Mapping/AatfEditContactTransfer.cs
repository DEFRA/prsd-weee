namespace EA.Weee.Web.ViewModels.Shared.Aatf.Mapping
{
    using System;
    using System.Collections.Generic;

    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;

    public class AatfEditContactTransfer
    {
        public AatfData AatfData { get; set; }

        public IList<CountryData> Countries { get; set; }

        public DateTime CurrentDate { get; set; }
    }
}