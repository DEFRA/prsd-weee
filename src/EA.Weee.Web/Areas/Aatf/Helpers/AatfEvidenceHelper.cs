namespace EA.Weee.Web.Areas.Aatf.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.AatfReturn;

    public class AatfEvidenceHelper : IAatfEvidenceHelper
    {
        public bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int? complianceYear)
        {
            var currentAatf = aatfs.FirstOrDefault(a => a.Id == aatfId);

            Condition.Requires(currentAatf).IsNotNull();

            //is there an aatf for the org in the selected compliance year that can create / edit notes
            var filter = aatfs.Where(a => a.AatfId == currentAatf.AatfId && a.CanCreateEditEvidence);

            return complianceYear.HasValue ? filter.Any(a => a.ComplianceYear == complianceYear) : filter.Any();
        }
    }
}