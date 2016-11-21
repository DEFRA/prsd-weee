namespace EA.Weee.RequestHandlers.Admin.GetSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Scheme;

    public class GetSubmissionChangesCsvDataAccess : IGetSubmissionChangesCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetSubmissionChangesCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task<MemberUpload> GetMemberUpload(Guid memberUploadId)
        {
            return context.MemberUploads
                .Where(m => m.Id == memberUploadId)
                .Include(m => m.Scheme)
                .SingleOrDefaultAsync();
        }

        public Task<List<SubmissionChangesCsvData>> GetSubmissionChanges(Guid memberUploadId)
        {
            return context.StoredProcedures.SpgSubmissionChangesCsvData(memberUploadId);
        }
    }
}
