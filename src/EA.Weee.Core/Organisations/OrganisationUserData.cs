namespace EA.Weee.Core.Organisations
{
    using Shared;
    using System;
    using Users;

    public class OrganisationUserData
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public UserData User { get; set; }

        public Guid OrganisationId { get; set; }

        public UserStatus UserStatus { get; set; }

        public OrganisationData Organisation { get; set; }
    }
}
