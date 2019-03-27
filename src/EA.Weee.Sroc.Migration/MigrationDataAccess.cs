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
    using RequestHandlers.Scheme.MemberRegistration;
    using Serilog;
    using Xml.MemberRegistration;

    public class MigrationDataAccess : IMigrationDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public IList<MemberUpload> FetchMemberUploadsToProcess()
        {
            var id = Guid.Parse("6b21d7ea-acbf-4266-8f1f-aa1400d5974d");
            var memberUploads = context.MemberUploads
                    .Include(m => m.ProducerSubmissions)
                    .Where(m => m.Id == id && m.IsSubmitted && m.InvoiceRun == null && m.ComplianceYear == 2019 && m.Scheme.CompetentAuthority.Abbreviation == "EA")
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

        public void UpdateProducerSubmissionAmount(Guid memberUploadId, string name, ProducerCharge producerCharge, statusType status)
        {
            var producersBymember = context.ProducerSubmissions
                .Where(p => p.MemberUploadId == memberUploadId);

            var producer = producersBymember.Where(p => p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Equals(name)
                            || (p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Equals(name))).ToList();
            
            if (producer.Count() != 1)
            {
                throw new ApplicationException(string.Format("Producer with name {0} in upload {1} could not be updated", name, memberUploadId));
            }

            Log.Information(string.Format("Producer charge for {0} updated from {1} to {2} and from band {3} to {4}", name, producer.First().ChargeThisUpdate, producerCharge.Amount, producer.First().ChargeBandAmount.ChargeBand, producerCharge.ChargeBandAmount.ChargeBand));
            
            producer.First().UpdateCharge(producerCharge.Amount, producerCharge.ChargeBandAmount, (int)status); 
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
