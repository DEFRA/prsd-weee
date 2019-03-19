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
    using OverrideImplementations;
    using Serilog;

    public class MigrationDataAccess : IMigrationDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public IList<MemberUpload> FetchMemberUploadsToProcess()
        {
            var memberUploads = context.MemberUploads
                    .Include(m => m.ProducerSubmissions)
                    .Where(m => m.IsSubmitted && m.InvoiceRun == null && m.ComplianceYear == 2019 && m.Scheme.CompetentAuthority.Abbreviation == "EA")
                    .OrderBy(m => m.SubmittedDate);

            return memberUploads.ToList();
        }

        public IList<MemberUpload> FetchMemberUploadsToRollback()
        {
            var memberUploads = context.MemberUploads
                .Where(m => m.IsSubmitted && m.InvoiceRun != null && m.ComplianceYear == 2019 && m.Scheme.CompetentAuthority.Abbreviation == "EA")
                .Include(m => m.ProducerSubmissions);

            return memberUploads.ToList();
        }

        public void UpdateMemberUploadAmount(MemberUpload memberUpload, decimal amount, bool hasAnnualCharge)
        {
            memberUpload.HasAnnualCharge = hasAnnualCharge;
            memberUpload.UpdateTotalCharges(amount);
        }

        public void ResetMemberUploadInvoice(MemberUpload memberUpload)
        {
            memberUpload.ResetInvoice();
        }

        public void UpdateProducerSubmissionAmount(Guid memberUploadId, string name, decimal amount)
        {
            var producer = context.ProducerSubmissions
                .Where(p => (p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Equals(name))
                            || (p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Equals(name))
                            && p.MemberUploadId == memberUploadId);

            if (producer.Count() != 1)
            {
                throw new ApplicationException(string.Format("Producer with name {0} in upload {1} could not be updated", name, memberUploadId));
            }

            Log.Information(string.Format("Producer charges for {0} updated from {1} to {2}", name, producer.First().ChargeThisUpdate, amount));

            producer.First().UpdateCharge(amount); 
        }

        public void ResetProducerSubmissionInvoice(IEnumerable<ProducerSubmission> producerSubmissions)
        {
            foreach (var producerSubmission in producerSubmissions)
            {
                producerSubmission.SetAsNotInvoiced();
            }
        }

        public void ResetMemberUploadsAnnualCharge(IEnumerable<MemberUpload> memberUploads)
        {
            foreach (var memberUpload in memberUploads)
            {
                context.MemberUploads.Find(memberUpload.Id).HasAnnualCharge = false;
            }
        }
    }
}
