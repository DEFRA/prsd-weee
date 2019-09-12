namespace EA.Weee.Requests.Admin.Aatf
{
    using System;

    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class EditAatfContact : IRequest<bool>
    {
        public AatfContactData ContactData { get; }

        public Guid AatfId { get; }

        public bool SendNotification { get; set; }

        public EditAatfContact(Guid aatfId, AatfContactData contactData)
        {
            this.ContactData = contactData;
            this.AatfId = aatfId;
        }
    }
}
