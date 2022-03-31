namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Evidence;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EvidenceDataAccess : IEvidenceDataAccess
    {
        private readonly WeeeContext context;

        public EvidenceDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Note> GetNoteById(Guid id)
        {
            return await context.Notes.FindAsync(id);
        }
    }
}
