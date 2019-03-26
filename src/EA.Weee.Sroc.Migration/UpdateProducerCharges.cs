namespace EA.Weee.Sroc.Migration
{
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using Core.Shared;
    using Domain.Scheme;
    using OverrideImplementations;
    using Serilog;
    using Weee.Requests.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.MemberRegistration;

    public class UpdateProducerCharges : IUpdateProducerCharges
    {
        private readonly IMigrationDataAccess memberUploadDataAccess;
        private readonly WeeeMigrationContext context;
        private readonly IXmlConverter xmlConverter;
        private readonly IMigrationProducerChargeCalculator producerChargeCalculator;
        private readonly IMigrationTotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;

        public UpdateProducerCharges(WeeeMigrationContext context,
            IMigrationDataAccess memberUploadDataAccess,
            IXmlConverter xmlConverter,
            IMigrationProducerChargeCalculator producerChargeCalculator,
            IMigrationTotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess)
        {
            this.memberUploadDataAccess = memberUploadDataAccess;
            this.xmlConverter = xmlConverter;
            this.producerChargeCalculator = producerChargeCalculator;
            this.totalChargeCalculatorDataAccess = totalChargeCalculatorDataAccess;
            this.context = context;
        }

        public void UpdateCharges()
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    var memberUploads = memberUploadDataAccess.FetchMemberUploadsToProcess();

                    memberUploadDataAccess.ResetMemberUploadsAnnualCharge(memberUploads);

                    context.SaveChanges();

                    foreach (var memberUpload in memberUploads)
                    {
                        Log.Information(string.Format("Process member upload {0}", memberUpload.Id));

                        var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data),
                            memberUpload.FileName);

                        var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));
                        var complianceYear = int.Parse(schemeType.complianceYear);
                        var scheme = context.Schemes.Single(c => c.OrganisationId == message.OrganisationId);

                        var hasAnnualCharge = totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, complianceYear, memberUpload.SubmittedDate.Value);
                        var annualChargedToBeAdded = !hasAnnualCharge && scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA;

                        var total = TotalCalculatedCharges(memberUpload, schemeType, annualChargedToBeAdded, scheme);

                        memberUploadDataAccess.UpdateMemberUploadAmount(memberUpload, total, annualChargedToBeAdded);

                        context.SaveChanges();

                        Log.Information(string.Format("Member upload {0} updated from {1} to {2}", memberUpload.Id, memberUpload.TotalCharges, total));
                    }

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
        }

        public void RollbackCharges()
        {
            try
            {
                var memberUploads = memberUploadDataAccess.FetchMemberUploadsToRollback();

                foreach (var memberUpload in memberUploads)
                {
                    memberUploadDataAccess.ResetProducerSubmissionInvoice(memberUpload.ProducerSubmissions);

                    memberUploadDataAccess.ResetMemberUploadInvoice(memberUpload);
                }

                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal TotalCalculatedCharges(MemberUpload memberUpload, schemeType schemeType, bool annualChargeToBeAdded, Scheme scheme)
        {
            var complianceYear = int.Parse(schemeType.complianceYear);
            var annualcharge = scheme.CompetentAuthority.AnnualChargeAmount ?? 0;
            var submittedDate = memberUpload.SubmittedDate.Value;

            var producerCharges = new Dictionary<string, ProducerCharge>();

            foreach (var producer in schemeType.producerList)
            {
                var producerName = producer.GetProducerName();

                var producerCharge = producerChargeCalculator.CalculateCharge(schemeType, producer, complianceYear, submittedDate);

                if (memberUpload.ProducerSubmissions.Any())
                {
                    memberUploadDataAccess.UpdateProducerSubmissionAmount(memberUpload.Id, producerName, producerCharge, producer.status);
                }

                if (!producerCharges.ContainsKey(producerName))
                {
                    producerCharges.Add(producerName, producerCharge);
                }
            }

            var totalCharges = 0m;
            totalCharges = producerCharges.Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            if (annualChargeToBeAdded && complianceYear > 2018 && scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
            {
                totalCharges = totalCharges + annualcharge;
            }

            return totalCharges;
        }
    }
}
