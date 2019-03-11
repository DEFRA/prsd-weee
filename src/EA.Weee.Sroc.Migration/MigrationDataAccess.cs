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

    public class MigrationDataAccess : IMigrationDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public async Task<IList<MemberUpload>> FetchMemberUploadsToProcess()
        {
            var memberUploads = context.MemberUploads
                    .Include(m => m.ProducerSubmissions)
                    .Where(m => m.IsSubmitted && m.InvoiceRun == null)
                    .OrderBy(m => m.SubmittedDate);

            return await memberUploads.ToListAsync();
        }

        public async Task<IList<MemberUpload>> FetchMemberUploadsToRollback()
        {
            var memberUploads = context.MemberUploads
                .Where(m => m.IsSubmitted && m.InvoiceRun != null && m.ComplianceYear == 2019 && m.Scheme.CompetentAuthority.Abbreviation == "EA")
                .Include(m => m.ProducerSubmissions);

            return await memberUploads.ToListAsync();
        }

        public async Task UpdateMemberUploadAmount(MemberUpload memberUpload, decimal amount)
        {
            //var memberUpload = context.MemberUploads.First(m => m.Id == id);

            await Task.Run(() => memberUpload.UpdateTotalCharges(amount));
        }

        public async Task ResetMemberUploadInvoice(MemberUpload memberUpload)
        {
            //var memberUpload = context.MemberUploads.First(m => m.Id == id);

            await Task.Run(() => memberUpload.InvoiceRun == null);
        }

        public async Task UpdateProducerSubmissionAmount(Guid memberUploadId, string name, decimal amount)
        {
            var producer = context.ProducerSubmissions
                .Where(p => p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Equals(name)
                            || p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Equals(name)
                            && p.MemberUploadId == memberUploadId);

            if (producer.Count() != 1)
            {
                throw new ApplicationException(string.Format("Producer with name {0} in upload {1} could not be updated", name, memberUploadId));
            }

            await Task.Run(() => producer.First().UpdateCharge(amount)); 
        }

        public async Task ResetProducerSubmissionInvoice(IEnumerable<ProducerSubmission> producerSubmissions)
        {
            await Task.Run(() => context.ProducerSubmissions.Where(c => producerSubmissions.Select(p => p.Id).Contains(c.Id))
                .ForEachAsync((c) => 
                {
                    c.SetAsNotInvoiced();
                }));
        }
    }
}
