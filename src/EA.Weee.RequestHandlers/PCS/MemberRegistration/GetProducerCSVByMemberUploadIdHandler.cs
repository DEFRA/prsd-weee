namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.PCS;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;

    internal class GetProducerCSVByMemberUploadIdHandler : IRequestHandler<GetProducerCSVByMemberUploadId, ProducerCSVFileData>
    {
        private readonly WeeeContext context;

        public GetProducerCSVByMemberUploadIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<ProducerCSVFileData> HandleAsync(GetProducerCSVByMemberUploadId message)
        {
            var memberUpload = await context.MemberUploads.SingleOrDefaultAsync(o => o.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentException(string.Format("Could not find member upload with id {0}",
                    message.MemberUploadId));
            }

            var csvData = memberUpload.Scheme.GetProducerCSV(memberUpload.ComplianceYear);
            var csvName = string.Format("{0:yyyy_MM_dd}", DateTime.Now) + " - " + memberUpload.ComplianceYear.ToString() + ".csv";
            var producerCSVFileData = new ProducerCSVFileData { FileContent = csvData, FileName = csvName };

            return producerCSVFileData;
        }
    }
}
