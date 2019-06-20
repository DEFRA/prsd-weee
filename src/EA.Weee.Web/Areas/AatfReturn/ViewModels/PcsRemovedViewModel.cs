namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;

    public class PcsRemovedViewModel
    {
        public List<SchemeData> RemovedSchemeList { get; set; }
        public Guid ReturnId { get; set; }
    }
}