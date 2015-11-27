namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Core.Shared;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetMemberUploadDataHandler : IRequestHandler<GetMemberUploadData, List<MemberUploadErrorData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IMap<MemberUploadError, MemberUploadErrorData> memberUploadErrorMap;

        public GetMemberUploadDataHandler(IWeeeAuthorization authorization, WeeeContext context, IMap<MemberUploadError, MemberUploadErrorData> memberUploadErrorMap)
        {
            this.authorization = authorization;
            this.context = context;
            this.memberUploadErrorMap = memberUploadErrorMap;
        }

        public async Task<List<MemberUploadErrorData>> HandleAsync(GetMemberUploadData message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.PcsId);

            var memberUpload = await context.MemberUploads.FirstOrDefaultAsync(m => m.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a MemberUpload with id {0}", message.MemberUploadId));
            }

            if (memberUpload.OrganisationId != message.PcsId)
            {
                throw new ArgumentException(string.Format("Member upload {0} is not owned by PCS {1}", message.MemberUploadId, message.PcsId));
            }

            return memberUpload.Errors.OrderBy(e => e.LineNumber).Select(e => memberUploadErrorMap.Map(e)).ToList();
        }
    }
}
