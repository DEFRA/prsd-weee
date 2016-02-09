namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
  
    public class UpdateOrganisationDetails : IRequest<bool>
    {
        public OrganisationData OrganisationData { get; private set; }

        public UpdateOrganisationDetails(OrganisationData organisationData)
        {
            OrganisationData = organisationData;
        }
    }
}
