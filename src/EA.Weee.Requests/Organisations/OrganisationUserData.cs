namespace EA.Weee.Requests.Organisations
{
    using System;

    public class OrganisationUserData
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid OrganisationId { get; set; }

        public OrganisationUserStatus OrganisationUserStatus { get; set; }

        public OrganisationData Organisation { get; set; }
    }
}
