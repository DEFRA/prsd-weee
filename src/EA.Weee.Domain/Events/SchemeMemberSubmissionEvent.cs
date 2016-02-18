namespace EA.Weee.Domain.Events
{
    using Prsd.Core.Domain;
    using Scheme;

    public class SchemeMemberSubmissionEvent : IEvent
    {
        public MemberUpload MemberUpload { get; private set; }

        public SchemeMemberSubmissionEvent(MemberUpload memberUpload)
        {
            MemberUpload = memberUpload;
        }
    }
}
