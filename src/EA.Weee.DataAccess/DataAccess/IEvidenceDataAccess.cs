namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Organisation;
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

        Task<IEnumerable<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes);

        Task<Guid> AddTransferNote(Organisation organisation,
            Scheme scheme,
            List<NoteTransferCategory> transferCategories,
            List<NoteTransferTonnage> transferTonnage,
            NoteStatus status,
            int complianceYear,
            string userId);

        Task<List<NoteTonnage>> GetTonnageByIds(List<Guid> ids);

        Task<int> GetComplianceYearByNotes(List<Guid> evidenceNoteIds);
    }
}