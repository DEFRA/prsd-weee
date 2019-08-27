namespace EA.Weee.Domain.Events
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;

    public class OrganisationUserRequestCompletedEvent : IEvent
    {
        public OrganisationUser OrganisationUser { get; private set; }

        public OrganisationUserRequestCompletedEvent(OrganisationUser organisationUser)
        {
            OrganisationUser = organisationUser;
        }
    }
}
