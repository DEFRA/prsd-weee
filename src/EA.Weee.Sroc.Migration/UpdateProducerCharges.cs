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
    using Weee.Requests.Scheme.MemberRegistration;

    public class UpdateProducerCharges : IUpdateProducerCharges
    {       
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IMigrationDataAccess memberUploadDataAccess;
        private readonly WeeeContext context;

        public UpdateProducerCharges(WeeeContext context, 
            IXMLChargeBandCalculator xmlChargeBandCalculator, 
            IMigrationDataAccess memberUploadDataAccess)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.memberUploadDataAccess = memberUploadDataAccess;
            this.context = context;
        }

        public async void Test()
        {
            var memberUploads = await memberUploadDataAccess.Fetch();

            foreach (var memberUpload in memberUploads)
            {
                var message = new ProcessXmlFile(memberUpload.OrganisationId, Encoding.ASCII.GetBytes(memberUpload.RawData.Data), memberUpload.FileName);

                decimal totalCharges = 0;
                var producerCharges = ProducerCharges(message, ref totalCharges);

                await memberUploadDataAccess.Update(memberUpload.Id, totalCharges);
            }

            await context.SaveChangesAsync();

            // what about uploads that have errors assume these ones would not have been submitted
        }

        private Dictionary<string, ProducerCharge> ProducerCharges(ProcessXmlFile message, ref decimal totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            return producerCharges;
        }
    }
}
