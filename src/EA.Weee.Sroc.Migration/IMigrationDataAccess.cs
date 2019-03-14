namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;

    public interface IMigrationDataAccess
    {
        IList<MemberUpload> FetchMemberUploadsToProcess();

        IList<MemberUpload> FetchMemberUploadsToRollback();

        void UpdateMemberUploadAmount(MemberUpload memberUpload, decimal amount, bool hasAnnualCharge);

        void ResetMemberUploadInvoice(MemberUpload memberUpload);

        void UpdateProducerSubmissionAmount(Guid memberUploadId, string name, decimal amount);

        void ResetProducerSubmissionInvoice(IEnumerable<ProducerSubmission> producerSubmissions);

        void ResetMemberUploadsAnnualCharge(IEnumerable<MemberUpload> memberUploads);
    }
}