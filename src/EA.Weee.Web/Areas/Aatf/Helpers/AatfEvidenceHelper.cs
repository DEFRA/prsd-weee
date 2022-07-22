namespace EA.Weee.Web.Areas.Aatf.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using EA.Weee.Core.AatfReturn;
    using Services;

    public class AatfEvidenceHelper : IAatfEvidenceHelper
    {
        private readonly ConfigurationService configurationService;

        public AatfEvidenceHelper(ConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        public bool AatfCanEditCreateNotes(List<AatfData> aatfs, Guid aatfId, int complianceYear)
        {
            var currentAatf = aatfs.FirstOrDefault(a => a.Id == aatfId);

            Condition.Requires(currentAatf).IsNotNull();

            //is there an aatf for the org in the selected compliance year that can create / edit notes
            return aatfs.Any(a => a.AatfId == currentAatf.AatfId && 
                                          a.CanCreateEditEvidence &&
                                          a.ComplianceYear == complianceYear);
        }

        public List<AatfData> GroupedValidAatfs(List<AatfData> aatfs)
        {
            return aatfs
                .Where(a => a.EvidenceSiteDisplay && a.ApprovalDate.HasValue && a.ApprovalDate.Value.Date > configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .GroupBy(a => a.AatfId)
                .Select(a => a.OrderByDescending(a1 => a1.ComplianceYear).First())
                .OrderBy(a => a.Name)
                .ToList();
        }
    }
}