namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;

    public interface IMigrationDataAccess
    {
        Task<IList<MemberUpload>> Fetch();

        Task UpdateMemberUpload(Guid id, decimal amount);

        Task<IList<ProducerSubmission>> FetchProducerSubmissionsByUpload(Guid id);
    }
}