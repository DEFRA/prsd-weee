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
    using Security;

    internal class GetMemberUploadByIdHandler : IRequestHandler<GetMemberUploadById, MemberUploadData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<MemberUpload, MemberUploadData> memberUploadMap;

        public GetMemberUploadByIdHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<MemberUpload, MemberUploadData> memberUploadMap)
        {
            this.authorization = authorization;
            this.context = context;
            this.memberUploadMap = memberUploadMap;
        }

        public async Task<MemberUploadData> HandleAsync(GetMemberUploadById message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var memberUpload = await context.MemberUploads.SingleOrDefaultAsync(m => m.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a MemberUpload with id {0}", message.MemberUploadId));
            }

            if (memberUpload.OrganisationId != message.OrganisationId)
            {
                throw new ArgumentException(string.Format("Member upload {0} is not owned by PCS {1}", message.MemberUploadId, message.OrganisationId));
            }

            return memberUploadMap.Map(memberUpload);
        }
    }
}
