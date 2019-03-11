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

        public async void UpdateCharges()
        {
            try
            {
                var memberUploads = await memberUploadDataAccess.FetchMemberUploadsToProcess();

                foreach (var memberUpload in memberUploads)
                {
                    var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data),
                        memberUpload.FileName);
                    var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

                    decimal totalCharges = 0;

                    var producerCharges = Calculate(memberUpload.Id, message, ref totalCharges);

                    await memberUploadDataAccess.UpdateMemberUploadAmount(memberUpload, totalCharges);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void RollbackCharges()
        {
            try
            {
                var memberUploads = await memberUploadDataAccess.FetchMemberUploadsToRollback();

                foreach (var memberUpload in memberUploads)
                {
                    await memberUploadDataAccess.ResetProducerSubmissionInvoice(memberUpload.ProducerSubmissions);

                    await memberUploadDataAccess.ResetMemberUploadInvoice(memberUpload);
                }

                await context.SaveChangesAsync();
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

                // match of trading name 
                if (producerCharge != null)
                {
                    if (!producerCharges.ContainsKey(producerName))
                    {
                        producerCharges.Add(producerName, producerCharge);
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Producer charge for producer {0} is null", producerName));
                }
            }

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            return producerCharges;
        }
    }
}
