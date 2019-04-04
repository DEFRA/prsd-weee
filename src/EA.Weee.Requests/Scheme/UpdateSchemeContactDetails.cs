namespace EA.Weee.Requests.Scheme
{
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class UpdateSchemeContactDetails : IRequest<bool>
    {
        public OrganisationData OrganisationData { get; private set; }

        public bool SendNotificationOnChange { get; set; }

        public UpdateSchemeContactDetails(OrganisationData organisationData, bool sendNotificationOnChange = false)
        {
            OrganisationData = organisationData;
            SendNotificationOnChange = sendNotificationOnChange;
        }
    }
}
