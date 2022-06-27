namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;

    public class GetAllNotes : IRequest<List<AdminEvidenceNoteData>>
    {
        public int SelectedComplianceYear { get; private set; }

        public GetAllNotes(int selectedComplianceYear)
        {
            SelectedComplianceYear = selectedComplianceYear;
        }
    }
}
