namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public interface IEvidenceDataAccess
    {
        Task<Note> GetNoteById(Guid id);

        Task<Note> Update(Note note, 
            Scheme recipient, 
            DateTime startDate, 
            DateTime endDate, 
            WasteType? wasteType, 
            Protocol? protocol,
            IList<NoteTonnage> tonnages,
            NoteStatus status);

        Task<List<Note>> GetAllNotes(EvidenceNoteFilter filter);

        Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId);

        Task<List<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes);
    }
}