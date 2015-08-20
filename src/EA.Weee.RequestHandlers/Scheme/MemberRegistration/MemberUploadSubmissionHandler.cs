﻿namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Security;

    internal class MemberUploadSubmissionHandler : IRequestHandler<MemberUploadSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;

        public MemberUploadSubmissionHandler(IWeeeAuthorization authorization, WeeeContext context)
        {
            this.authorization = authorization;
            this.context = context;
        }

        public async Task<Guid> HandleAsync(MemberUploadSubmission message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var memberUpload = await context.MemberUploads.SingleOrDefaultAsync(m => m.Id == message.MemberUploadId);

            if (memberUpload == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a MemberUpload with id {0}", message.MemberUploadId));
            }

            if (!memberUpload.IsSubmitted)
            {
                memberUpload.Submit();
                await context.SaveChangesAsync();
            }

            return memberUpload.Id;
        }
    }
}
