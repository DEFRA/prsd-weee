namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using EA.Weee.Domain.AatfReturn;

    public interface IEvidenceDataAccess
    {
        Task<Note> GetNoteById(Guid id);

        Task<Note> Update(Note note, 
            Organisation recipient, 
            DateTime startDate, 
            DateTime endDate, 
            WasteType? wasteType, 
            Protocol? protocol,
            IList<NoteTonnage> tonnages,
            NoteStatus status,
            DateTime date);

        Task<EvidenceNoteResults> GetAllNotes(NoteFilter filter);

        Task<IEnumerable<int>> GetComplianceYearsForNotes(List<int> allowedStatuses);

        Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId, int complianceYear);

        Task<EvidenceNoteResults> GetNotesToTransfer(Guid recipientOrganisationId, 
            List<int> categories,
            List<Guid> excludeEvidenceNotes,
            int complianceYear,
            string searchRef,
            int pageNumber,
            int pageSize);

        Task<EvidenceNoteResults> GetTransferSelectedNotes(Guid recipientOrganisationId, 
            List<Guid> evidenceNotes,
            List<int> categories);

        Task<Note> AddTransferNote(Organisation organisation,
            Organisation scheme,
            List<NoteTransferTonnage> transferTonnage,
            NoteStatus status,
            int complianceYear,
            string userId,
            DateTime date);

        Task<List<NoteTonnage>> GetTonnageByIds(List<Guid> ids);

        Task<Note> UpdateTransfer(Note note, Organisation recipient,
            IList<NoteTransferTonnage> tonnages,
            NoteStatus status,
            DateTime updateDate);

        Task<List<Organisation>> GetRecipientOrganisations(Guid? aatfId, int complianceYear, List<NoteStatus> allowedStatus, List<NoteType> allowedNoteTypes);

        Task<List<Organisation>> GetTransferOrganisations(int complianceYear, List<NoteStatus> allowedStatus, List<NoteType> allowedNoteTypes);

        Task<bool> HasApprovedWasteHouseHoldEvidence(Guid recipientId, int complianceYear);

        Task<List<Aatf>> GetAatfForAllNotesForComplianceYear(int complianceYear, List<NoteStatus> allowedStatus);

        Note DeleteZeroTonnageFromSubmittedTransferNote(Note note, NoteStatus status, NoteType type);
    }
}