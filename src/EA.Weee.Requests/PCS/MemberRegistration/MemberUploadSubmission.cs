namespace EA.Weee.Requests.PCS.MemberRegistration
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
