﻿namespace EA.Weee.Sroc.Migration
{
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        private readonly IMigrationProducerChargeBandCalculatorChooser producerChargeCalculator;
        private readonly IMigrationTotalChargeCalculatorDataAccess totalChargeCalculatorDataAccess;

        public UpdateProducerCharges(WeeeMigrationContext context,
            IMigrationDataAccess memberUploadDataAccess,
            IXmlConverter xmlConverter,
            IMigrationProducerChargeBandCalculatorChooser producerChargeCalculator,
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

                    //var ids = new List<Guid>() { Guid.Parse("12EF32AD-44F8-4720-A96C-A9A600CE2593"), Guid.Parse("72FA232F-B1ED-4F5F-AACA-A9FC00D7B806"), Guid.Parse("D31B4E6D-B490-48E5-96FC-AA1E00F0595A") };
                    foreach (var memberUpload in memberUploads)
                    {
                        //.Where(m => ids.Contains(m.Id)))
                        var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data), memberUpload.FileName);

                        var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));
                        var complianceYear = int.Parse(schemeType.complianceYear);
                        var scheme = context.Schemes.Single(c => c.OrganisationId == message.OrganisationId);

                        var hasAnnualCharge = totalChargeCalculatorDataAccess.CheckSchemeHasAnnualCharge(scheme, complianceYear, memberUpload.SubmittedDate.Value);
                        var annualChargedToBeAdded = !hasAnnualCharge && scheme.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA;

                        var total = TotalCalculatedCharges(memberUpload, schemeType, annualChargedToBeAdded, scheme);

                        Log.Information(string.Format("Member upload {0} updated from {1} to {2}", memberUpload.Id, memberUpload.TotalCharges, total));

                        memberUploadDataAccess.UpdateMemberUploadAmount(memberUpload, total, annualChargedToBeAdded);                       
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
                
                var producerCharge = producerChargeCalculator.GetProducerChargeBand(schemeType, producer, memberUpload, producerName);

                if (memberUpload.ProducerSubmissions.Any())
                {
                    if (producer.status == statusType.A)
                    {
                        Log.Information(string.Format("Producer {0}", producer.registrationNo));
                    }
                    memberUploadDataAccess.UpdateProducerSubmissionAmount(memberUpload.Id, producerName, producerCharge, producer.status, producer);
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
                totalCharges = (totalCharges + annualcharge);
            }

            return totalCharges;
        }
    }
}
