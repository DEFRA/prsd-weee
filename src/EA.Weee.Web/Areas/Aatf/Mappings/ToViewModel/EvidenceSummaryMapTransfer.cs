namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;

    public class EvidenceSummaryMapTransfer
    {
        public Guid OrganisationId { get; private set; }

        public Guid AatfId { get; private set; }

        public AatfEvidenceSummaryData AatfEvidenceSummaryData { get; private set; }

        public EvidenceSummaryMapTransfer(Guid organisationId, Guid aatfId, AatfEvidenceSummaryData summaryData)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(aatfId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(summaryData).IsNotNull();

            OrganisationId = organisationId;
            AatfId = aatfId;
            AatfEvidenceSummaryData = summaryData;
        }
    }
}