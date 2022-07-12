namespace EA.Weee.Web.Areas.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using CuttingEdge.Conditions;

    public class AatfEvidenceHelper : IAatfEvidenceHelper
    {
        public bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int complianceYear)
        {
            var currentAatf = aatfs.FirstOrDefault(a => a.Id == aatfId);

            Condition.Requires(currentAatf).IsNotNull();

            //is there an aatf for the org in the selected compliance year that can create / edit notes
            return aatfs.Any(a =>
                a.AatfId == currentAatf.AatfId && a.ComplianceYear == complianceYear && a.CanCreateEditEvidence);
        }
    }
}