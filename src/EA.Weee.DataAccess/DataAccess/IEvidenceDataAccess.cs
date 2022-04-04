namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Threading.Tasks;

    public interface IEvidenceDataAccess
    {
        Task<Note> GetNoteById(Guid id);
    }
}