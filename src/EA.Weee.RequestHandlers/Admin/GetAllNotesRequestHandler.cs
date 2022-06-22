namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.Admin;

    public class GetAllNotesRequestHandler : IRequestHandler<GetAllNotes, List<EvidenceNoteData>>
    {
    }
}
