namespace EA.Weee.Web.Areas.Aatf
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface IAatfEvidenceHelper
    {
        bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int complianceYear);
    }
}