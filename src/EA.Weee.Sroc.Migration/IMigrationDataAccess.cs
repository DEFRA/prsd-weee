namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;

    public interface IMigrationDataAccess
    {
        Task<IList<MemberUpload>> FetchMemberUploadsToProcess();

        Task<IList<MemberUpload>> FetchMemberUploadsToRollback();

        Task UpdateMemberUploadAmount(MemberUpload memberUpload, decimal amount);

        Task ResetMemberUploadInvoice(MemberUpload memberUpload);

        Task UpdateProducerSubmissionAmount(Guid memberUploadId, string name, decimal amount);

        Task ResetProducerSubmissionInvoice(IEnumerable<ProducerSubmission> producerSubmissions);
    }
}