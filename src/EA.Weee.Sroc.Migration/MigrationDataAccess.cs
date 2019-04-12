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
            var memberUploads = context.MemberUploads
                    .Include(m => m.ProducerSubmissions)
                    .Where(m => m.IsSubmitted && m.InvoiceRun == null && m.ComplianceYear == 2019 && m.Scheme.CompetentAuthority.Abbreviation == "EA")
                    .OrderBy(m => m.CreatedDate);

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

            context.SaveChanges();
        }

        public void ResetMemberUploadInvoice(MemberUpload memberUpload)
        {
            memberUpload.ResetInvoice();
        }

        public void UpdateProducerSubmissionAmount(Guid memberUploadId, string name, ProducerCharge producerCharge, statusType status, producerType producerType)
        {
            var producersMember = context.ProducerSubmissions
                .Where(p => p.MemberUploadId == memberUploadId).ToList();

            var producer = producersMember.Where(p => p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Trim().Equals(name.Trim())
                            || (p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Trim().Equals(name.Trim()))).ToList();
            
            Log.Information("number found {0}", producer.Count);
            if (producer.Count() == 1)
            {
                //throw new ApplicationException(string.Format("Producer with name {0} in upload {1} could not be updated", name, memberUploadId));
                Log.Information(string.Format("Producer charge for {0} updated from {1} to {2} and from band {3} to {4}", name, producer.First().ChargeThisUpdate, producerCharge.Amount, producer.First().ChargeBandAmount.ChargeBand, producerCharge.ChargeBandAmount.ChargeBand));

                producer.First().UpdateCharge(producerCharge.Amount, producerCharge.ChargeBandAmount, (int)status);

                context.SaveChanges();
            }
            else
            {
                Log.Information(string.Format("last chance to find user {0}", producerType.tradingName));

                var company = producerType.producerBusiness.Item; 

                var findProducer = new List<ProducerSubmission>();
                if (company.GetType() == typeof(companyType))
                {
                    Log.Information(string.Format("1. {0}", ((companyType)company).companyName));

                    var test = context.ProducerSubmissions
                        .Where(p => p.MemberUploadId == memberUploadId).Where(p => p.ProducerBusiness.CompanyDetails != null);

                    foreach (var producerSubmission in test)
                    {
                        Log.Information(string.Format("{0}", producerSubmission.ProducerBusiness.CompanyDetails.Name));
                    }
                    findProducer = context.ProducerSubmissions
                        .Where(p => p.MemberUploadId == memberUploadId && p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Trim().Equals(((companyType)company).companyName.Trim())).ToList();
                }
                else if (company.GetType() == typeof(partnershipType))
                {
                    Log.Information(string.Format("2. {0}", ((partnershipType)company).partnershipName));
                    findProducer = context.ProducerSubmissions
                        .Where(p => p.MemberUploadId == memberUploadId && p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Trim().Equals(((partnershipType)company).partnershipName.Trim())).ToList();
                }

                if (producer.Count() == 1)
                {
                    Log.Information(string.Format("Producer charge for {0} updated from {1} to {2} and from band {3} to {4}", name, producer.First().ChargeThisUpdate, producerCharge.Amount, producer.First().ChargeBandAmount.ChargeBand, producerCharge.ChargeBandAmount.ChargeBand));

                    findProducer.First().UpdateCharge(producerCharge.Amount, producerCharge.ChargeBandAmount, (int)status);

                    context.SaveChanges();
                }
                else
                {
                    Log.Information(string.Format("Could not find {0}", name));
                }
            }
            //Log.Information(string.Format("Producer charge for {0} updated from {1} to {2} and from band {3} to {4}", name, producer.First().ChargeThisUpdate, producerCharge.Amount, producer.First().ChargeBandAmount.ChargeBand, producerCharge.ChargeBandAmount.ChargeBand));
            
            //producer.First().UpdateCharge(producerCharge.Amount, producerCharge.ChargeBandAmount, (int)status);

            //context.SaveChanges();
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
