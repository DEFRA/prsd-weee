namespace EA.Weee.Domain.Events
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MemberUploadSubmittedEvent : IEvent
    {
        public MemberUpload MemberUpload { get; private set; }

        public MemberUploadSubmittedEvent(MemberUpload memberUpload)
        {
            MemberUpload = memberUpload;
        }
    }
}
