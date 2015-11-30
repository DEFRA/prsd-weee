namespace EA.Weee.Domain.Events
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Scheme;

    public class MemberUploadSubmittedEvent : IEvent
    {
        public MemberUpload MemberUpload { get; private set; }

        public MemberUploadSubmittedEvent(MemberUpload memberUpload)
        {
            MemberUpload = memberUpload;
        }
    }
}
