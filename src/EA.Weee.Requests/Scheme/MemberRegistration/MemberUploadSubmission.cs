namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class MemberUploadSubmission : IRequest<Guid>
    {
        public Guid MemberUploadId { get; private set; }

        public MemberUploadSubmission(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
