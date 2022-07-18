namespace EA.Weee.Web.Areas.Aatf.Helpers
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;

    public interface IAatfEvidenceHelper
    {
        bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int complianceYear);
    }
}