namespace EA.Weee.Domain.Events
{
    using System;
    using Prsd.Core.Domain;
   
    public class OrganisationUserRequestEvent : IEvent
    {
        public Guid OrganisationId { get; private set; }
        public Guid UserId { get; private set; }

        internal OrganisationUserRequestEvent(Guid organisationId, Guid userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}
