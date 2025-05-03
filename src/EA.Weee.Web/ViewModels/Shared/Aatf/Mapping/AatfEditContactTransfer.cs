namespace EA.Weee.Web.ViewModels.Shared.Aatf.Mapping
{
    using Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;

    public class AatfEditContactTransfer
    {
        public AatfData AatfData { get; set; }

        public IList<CountryData> Countries { get; set; }

        public DateTime CurrentDate { get; set; }
    }
}