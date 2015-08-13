namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;

    internal class MemberUploadSubmissionHandler : IRequestHandler<MemberUploadSubmission, Guid>
    {
        private readonly WeeeContext context;

        public MemberUploadSubmissionHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(MemberUploadSubmission message)
        {
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
