namespace EA.Weee.Core.Organisations
{
    using System;
    using Shared;

    public class OrganisationUserData
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid OrganisationId { get; set; }

        public UserStatus UserStatus { get; set; }

        public OrganisationData Organisation { get; set; }
    }
}
