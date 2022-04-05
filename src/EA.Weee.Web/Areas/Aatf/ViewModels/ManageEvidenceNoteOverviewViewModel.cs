namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    public abstract class ManageEvidenceNoteOverviewViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
