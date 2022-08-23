namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Prsd.Core;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewEvidenceSummaryViewModelMapTransfer : IObligationEvidenceSummaryBase
    {
        public Guid OrganisationId { get; protected set; }

        public ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; set; }

        public SchemePublicInfo Scheme { get; protected set; }
        
        public DateTime CurrentDate { get; protected set; }

        public int ComplianceYear { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; protected set; } 

        public ViewEvidenceSummaryViewModelMapTransfer(Guid organisationId,
            ObligationEvidenceSummaryData obligationEvidenceSummaryData,
            ManageEvidenceNoteViewModel manageEvidenceNoteViewModel,
            SchemePublicInfo scheme, 
            DateTime currentDate, int complianceYear)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);

            OrganisationId = organisationId;
            ObligationEvidenceSummaryData = obligationEvidenceSummaryData;
            ManageEvidenceNoteViewModel = manageEvidenceNoteViewModel;
            Scheme = scheme;
            CurrentDate = currentDate;
            ComplianceYear = complianceYear;
        }
    }
}
