namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;

    internal class GetMemberUploadByIdHandler : IRequestHandler<GetMemberUploadById, MemberUploadData>
    {
        private readonly WeeeContext context;

        private readonly IMap<MemberUpload, MemberUploadData> memberUploadMap;

        public GetMemberUploadByIdHandler(WeeeContext context, IMap<MemberUpload, MemberUploadData> memberUploadMap)
        {
            this.context = context;
            this.memberUploadMap = memberUploadMap;
        }

        public async Task<MemberUploadData> HandleAsync(GetMemberUploadById message)
        {
            var memberUpload = await context.MemberUploads.SingleOrDefaultAsync(m => m.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a MemberUpload with id {0}", message.MemberUploadId));
            }

            return memberUploadMap.Map(memberUpload);
        }
    }
}
