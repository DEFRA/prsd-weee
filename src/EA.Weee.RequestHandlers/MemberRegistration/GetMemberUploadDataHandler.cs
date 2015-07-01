namespace EA.Weee.RequestHandlers.MemberRegistration
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.MemberRegistration;
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

            return memberUpload.Errors.Select(e => memberUploadErrorMap.Map(e)).ToList();
        }
    }
}
