namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
  
    public class UpdateOrganisationContactDetails : IRequest<bool>
    {
        public OrganisationData OrganisationData { get; private set; }

        public UpdateOrganisationContactDetails(OrganisationData organisationData)
        {
            OrganisationData = organisationData;
        }
    }
}
