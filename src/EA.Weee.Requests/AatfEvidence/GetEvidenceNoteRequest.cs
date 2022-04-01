namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetEvidenceNoteRequest : IRequest<EvidenceNoteData>
    {
        public Guid EvidenceNoteId { get; private set; }
        
        public Guid AatfId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public GetEvidenceNoteRequest(Guid evidenceNoteId, Guid organisationId)
        {
            // all checkings will be done in base class
            Guard.ArgumentNotDefaultValue(() => organisationId, evidenceNoteId);
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);

            EvidenceNoteId = evidenceNoteId;
            OrganisationId = organisationId;

            //EvidenceDataAccess da = new EvidenceDataAccess();
            /*
             * SELECT  Evidence.Note.AatfId, Evidence.Note.OrganisationId, Evidence.Note.StartDate, Evidence.Note.EndDate, Evidence.Note.RecipientId, Evidence.Note.WasteType, Evidence.Note.Protocol, Evidence.Note.Status, 
                       Evidence.Note.Id AS Expr4, Evidence.NoteTonnage.NoteId, Evidence.NoteTonnage.CategoryId, Evidence.NoteTonnage.Received, Evidence.NoteTonnage.Reused, Evidence.Note.Reference
                FROM   Evidence.Note INNER JOIN
                       Evidence.NoteTonnage ON Evidence.Note.Id = Evidence.NoteTonnage.NoteId
             * 
             * DID NOT FIND WHERE TO PLACE MY STORED PROC / VIEW - TBD
             */
        }
    }
}
