namespace EA.Weee.RequestHandlers.Admin.GetSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using Domain.Scheme;

    public interface IGetSubmissionChangesCsvDataAccess
    {
        Task<MemberUpload> GetMemberUpload(Guid memberUploadId);

        Task<List<SubmissionChangesCsvData>> GetSubmissionChanges(Guid memberUploadId);
    }
}
