namespace EA.Weee.Domain
{
    using System;
    using EA.Prsd.Core.Domain;

    public class OrganisationUser : Entity
    {
        public string UserId { get; private set; }

        public Guid OrganisationId { get; private set; }

        public OrganisationUserStatus UserStatus { get; private set; }

        protected OrganisationUser()
        {
        }

        public OrganisationUser(Guid userId, Guid organisationId, OrganisationUserStatus userStatus)
        {
            UserId = userId.ToString();
            OrganisationId = organisationId;
            UserStatus = userStatus;
        }
    }
}