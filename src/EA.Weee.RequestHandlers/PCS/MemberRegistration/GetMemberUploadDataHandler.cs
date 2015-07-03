namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Requests.Shared;

    internal class GetMemberUploadDataHandler : IRequestHandler<GetMemberUploadData, List<MemberUploadErrorData>>
    {
        private readonly WeeeContext context;

        private readonly IMap<MemberUploadError, MemberUploadErrorData> memberUploadErrorMap;

        public GetMemberUploadDataHandler(WeeeContext context, IMap<MemberUploadError, MemberUploadErrorData> memberUploadErrorMap)
        {
            this.context = context;
            this.memberUploadErrorMap = memberUploadErrorMap;
        }

        public async Task<List<MemberUploadErrorData>> HandleAsync(GetMemberUploadData message)
        {
            var memberUpload = await context.MemberUploads.FirstOrDefaultAsync(m => m.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a MemberUpload with id {0}", message.MemberUploadId));
            }

            return memberUpload.Errors.Select(e => memberUploadErrorMap.Map(e)).ToList();
        }
    }
}
