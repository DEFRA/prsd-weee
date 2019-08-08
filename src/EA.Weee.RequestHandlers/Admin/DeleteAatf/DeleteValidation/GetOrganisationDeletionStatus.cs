namespace EA.Weee.RequestHandlers.Admin.DeleteAatf.DeleteValidation
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess.DataAccess;

    public class GetOrganisationDeletionStatus : IGetOrganisationDeletionStatus
    {
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetOrganisationDeletionStatus(IOrganisationDataAccess organisationDataAccess)
        {
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<CanOrganisationBeDeletedFlags> Validate(Guid organisationId, int year)
        {
            var result = new CanOrganisationBeDeletedFlags();

            var organisationHasReturns = await organisationDataAccess.HasReturns(organisationId, year);

            if (organisationHasReturns)
            {
                result |= CanOrganisationBeDeletedFlags.HasReturns;
            }

            if (await organisationDataAccess.HasActiveUsers(organisationId))
            {
                result |= CanOrganisationBeDeletedFlags.HasActiveUsers;
            }

            return result;
        }
    }
}
