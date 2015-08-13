namespace EA.Weee.Core.Organisations
{
    using System;
    using NewUser;
    using Shared;

    public class OrganisationUserData
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public UserData User { get; set; }

        public Guid OrganisationId { get; set; }

        public UserStatus OrganisationUserStatus { get; set; }

        public OrganisationData Organisation { get; set; }
    }
}
