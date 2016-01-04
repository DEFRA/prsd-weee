namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Security;
    using Shared.DomainUser;

    internal class MemberUploadSubmissionHandler : IRequestHandler<MemberUploadSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IDomainUserContext domainUserContext;

        public MemberUploadSubmissionHandler(
            IWeeeAuthorization authorization,
            WeeeContext context,
            IDomainUserContext domainUserContext)
        {
            this.authorization = authorization;
            this.context = context;
            this.domainUserContext = domainUserContext;
        }

        public async Task<Guid> HandleAsync(MemberUploadSubmission message)
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

            if (!memberUpload.IsSubmitted)
            {
                User submittingUser = await domainUserContext.GetCurrentUserAsync();

                memberUpload.Submit(submittingUser);
                await context.SaveChangesAsync();
            }

            return memberUpload.Id;
        }
    }
}
