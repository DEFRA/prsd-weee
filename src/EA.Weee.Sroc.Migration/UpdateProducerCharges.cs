namespace EA.Weee.Sroc.Migration
{
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Domain.Error;
    using Domain.Scheme;
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

        public async void Test()
        {
            var memberUploads = await memberUploadDataAccess.Fetch();

            foreach (var memberUpload in memberUploads)
            {
                var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data), memberUpload.FileName);
                var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

                decimal totalCharges = 0;

                var producerCharges = Calculate(message, ref totalCharges);

                //foreach (var producerCharge in producerCharges)
                //{
                //    // set the producer charge status here based on if update or not?
                //    //producerChargeCalculator.CalculateCharge(schemeType.approvalNo, producer, complianceYear);
                //}

                await memberUploadDataAccess.UpdateMemberUpload(memberUpload.Id, totalCharges);
            }

            //await context.SaveChangesAsync();

            // what about uploads that have errors assume these ones would not have been submitted
        }

        //private Dictionary<string, ProducerCharge> ProducerCharges(ProcessXmlFile message, ref decimal totalCharges)
        //{
        //    var producerCharges = xmlChargeBandCalculator.Calculate(message);

        //    if (xmlChargeBandCalculator.ErrorsAndWarnings.Any())
        //    {
        //        throw new Exception(string.Join(", ", xmlChargeBandCalculator.ErrorsAndWarnings));
        //    }

        //    totalCharges = producerCharges
        //        .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

        //    return producerCharges;
        //}

        public Dictionary<string, ProducerCharge> Calculate(ProcessXmlFile message, ref decimal totalCharges)
        {
            var schemeType = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

            var producerCharges = new Dictionary<string, ProducerCharge>();
            var complianceYear = Int32.Parse(schemeType.complianceYear);

            foreach (var producer in schemeType.producerList)
            {
                var producerName = producer.GetProducerName();

                // if the producer has a PRN and the trading name exists in the database for the member upload set the status to A?
                var producerCharge = producerChargeCalculator.CalculateCharge(schemeType.approvalNo, producer, complianceYear);

                //producer.tradingName`   
                // match of trading name 
                if (producerCharge != null)
                {
                    if (!producerCharges.ContainsKey(producerName))
                    {
                        producerCharges.Add(producerName, producerCharge);
                    }
                    else
                    {
                        throw new Exception();
                        //ErrorsAndWarnings.Add(
                        //    new MemberUploadError(
                        //        ErrorLevel.Error,
                        //        UploadErrorType.Business,
                        //        string.Format(
                        //            "We are unable to check for warnings associated with the charge band of the producer {0} until the duplicate name has been fixed.",
                        //            producerName)));
                    }
                }
            }

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            return producerCharges;
        }
    }
}
