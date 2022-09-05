namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using System.Security.Principal;
    using Core.AatfEvidence;
    using Prsd.Core;

    public class ViewEvidenceNoteMapTransfer
    {
        public Guid SchemeId { get; set; }

        public EvidenceNoteData EvidenceNoteData { get; private set; }

        public object NoteStatus { get; private set; }

        public bool IncludeAllCategories { get; set; }

        public bool PrintableVersion { get; set; }

        public IPrincipal User { get; private set; }

        public ViewEvidenceNoteMapTransfer(EvidenceNoteData evidenceNoteData, object noteStatus, bool printableVersion, IPrincipal user = null)
        {
            Guard.ArgumentNotNull(() => evidenceNoteData, evidenceNoteData);

            EvidenceNoteData = evidenceNoteData;
            NoteStatus = noteStatus;
            IncludeAllCategories = true;
            User = user;
            PrintableVersion = printableVersion;
        }

        public string RedirectTab { get; set; }
    }
}