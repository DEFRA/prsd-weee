namespace EA.Weee.Requests.Admin
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    public class GetAllNotes : IRequest<Guid>
    {
        public NoteType NoteType { get; set; }

        public int SelectedComplianceYear { get; set; }
    }
}
