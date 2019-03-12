namespace EA.Weee.Sroc.Migration
{
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OverrideImplementations;
    using Weee.Requests.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.MemberRegistration;

    public class UpdateProducerCharges : IUpdateProducerCharges
    {
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IMigrationDataAccess memberUploadDataAccess;
        private readonly WeeeMigrationContext context;
        private readonly IXmlConverter xmlConverter;
        private readonly IProducerChargeCalculator producerChargeCalculator;

        public UpdateProducerCharges(WeeeMigrationContext context,
            IXMLChargeBandCalculator xmlChargeBandCalculator,
            IMigrationDataAccess memberUploadDataAccess,
            IXmlConverter xmlConverter,
            IProducerChargeCalculator producerChargeCalculator)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.memberUploadDataAccess = memberUploadDataAccess;
            this.xmlConverter = xmlConverter;
            this.producerChargeCalculator = producerChargeCalculator;
            this.context = context;
        }

        public void UpdateCharges()
        {
            try
            {
                var memberUploads = memberUploadDataAccess.FetchMemberUploadsToProcess();

                foreach (var memberUpload in memberUploads)
                {
                    var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data), memberUpload.FileName);
                    var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

                    decimal totalCharges = 0;

                    var producerCharges = Calculate(memberUpload.Id, message, ref totalCharges);

                    memberUploadDataAccess.UpdateMemberUploadAmount(memberUpload, totalCharges);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
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

        public Dictionary<string, ProducerCharge> Calculate(Guid memberUploadId, ProcessXmlFile message, ref decimal totalCharges)
        {
            var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

            var producerCharges = new Dictionary<string, ProducerCharge>();
            var complianceYear = Int32.Parse(schemeType.complianceYear);

            foreach (var producer in schemeType.producerList)
            {
                var producerName = producer.GetProducerName();

                // if the producer has a PRN and the trading name exists in the database for the member upload set the status to A?
                var producerCharge = producerChargeCalculator.CalculateCharge(schemeType.approvalNo, producer, complianceYear);

                memberUploadDataAccess.UpdateProducerSubmissionAmount(memberUploadId, producerName, producerCharge.Amount);

                if (!producerCharges.ContainsKey(producerName))
                {
                    producerCharges.Add(producerName, producerCharge);
                }                
            }

            totalCharges = producerCharges.Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            return producerCharges;
        }
    }
}
