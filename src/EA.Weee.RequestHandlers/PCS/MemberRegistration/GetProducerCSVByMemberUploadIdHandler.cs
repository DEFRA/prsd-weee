namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;

    internal class GetProducerCSVByMemberUploadIdHandler : IRequestHandler<GetProducerCSVByMemberUploadId, string>
    {
        private readonly WeeeContext context;

        public GetProducerCSVByMemberUploadIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<string> HandleAsync(GetProducerCSVByMemberUploadId message)
        {
            var memberUpload = await context.MemberUploads.SingleOrDefaultAsync(o => o.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentException(string.Format("Could not find member upload with id {0}",
                    message.MemberUploadId));
            }

            var csvData = memberUpload.Scheme.GetProducerCSV(memberUpload.ComplianceYear);

            return csvData;
        }
    }
}
