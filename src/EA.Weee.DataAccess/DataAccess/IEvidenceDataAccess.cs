﻿namespace EA.Weee.DataAccess.DataAccess
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
            NoteStatus status,
            DateTime date);

        Task<List<Note>> GetAllNotes(NoteFilter filter);

        Task<int> GetNoteCountByStatusAndAatf(NoteStatus status, Guid aatfId, int complianceYear);

        Task<IEnumerable<Note>> GetNotesToTransfer(Guid schemeId, List<int> categories, List<Guid> evidenceNotes);

        Task<Guid> AddTransferNote(Organisation organisation,
            Scheme scheme,
            List<NoteTransferTonnage> transferTonnage,
            NoteStatus status,
            int complianceYear,
            string userId,
            DateTime date);

        Task<List<NoteTonnage>> GetTonnageByIds(List<Guid> ids);

        Task<int> GetComplianceYearByNotes(List<Guid> evidenceNoteIds);
    }
}