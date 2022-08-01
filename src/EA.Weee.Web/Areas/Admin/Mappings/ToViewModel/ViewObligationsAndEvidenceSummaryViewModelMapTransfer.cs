namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.Admin.Obligation;
    using System;

    public class ViewObligationsAndEvidenceSummaryViewModelMapTransfer
    {
        public Guid OrganisationId { get; private set; }

        public Guid PcsId { get; private set; }

        public ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; private set; }

        public ViewObligationsAndEvidenceSummaryViewModelMapTransfer(Guid organisationId, Guid pcsId, ObligationEvidenceSummaryData obligationEvidenceSummaryData)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(pcsId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(obligationEvidenceSummaryData).IsNotNull();

            OrganisationId = organisationId;
            PcsId = pcsId;
            ObligationEvidenceSummaryData = obligationEvidenceSummaryData;
        }
    }
}