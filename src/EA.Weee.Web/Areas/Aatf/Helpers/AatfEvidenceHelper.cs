namespace EA.Weee.Web.Areas.Aatf.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.AatfReturn;

    public class AatfEvidenceHelper : IAatfEvidenceHelper
    {
        public bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int complianceYear)
        {
            var currentAatf = aatfs.FirstOrDefault(a => a.Id == aatfId);

            Condition.Requires(currentAatf).IsNotNull();

            //is there an aatf for the org in the selected compliance year that can create / edit notes
            return aatfs.Any(a => a.AatfId == currentAatf.AatfId && 
                                          a.CanCreateEditEvidence &&
                                          a.ComplianceYear == complianceYear);
        }
    }
}