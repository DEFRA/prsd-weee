namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Producer;
    using Domain.Scheme;
    using Domain.User;

    public class MigrationDataAccess : IMigrationDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public async Task<IList<MemberUpload>> Fetch()
        {
            var memberUploads = context.MemberUploads
                    .Include(m => m.ProducerSubmissions)
                .Where(m => m.IsSubmitted)
                .OrderBy(m => m.SubmittedDate);

            return await memberUploads.ToListAsync();
        }

        public async Task UpdateMemberUpload(Guid id, decimal amount)
        {
            var memberUpload = context.MemberUploads.First(m => m.Id == id);

            await Task.Run(() => memberUpload.UpdateTotalCharges(amount));
        }

        public async Task<IList<ProducerSubmission>> FetchProducerSubmissionsByUpload(Guid id)
        {
            return await Task.Run(() => new List<ProducerSubmission>());
        }
    }
}
