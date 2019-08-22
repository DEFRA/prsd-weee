namespace EA.Weee.RequestHandlers.Admin.GetSubmissionChanges
{
    using DataAccess.StoredProcedure;
    using Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetSubmissionChangesCsvDataAccess
    {
        Task<MemberUpload> GetMemberUpload(Guid memberUploadId);

        Task<List<SubmissionChangesCsvData>> GetSubmissionChanges(Guid memberUploadId);
    }
}
