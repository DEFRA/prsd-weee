namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;

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

            if (await organisationDataAccess.HasReturns(organisationId, year))
            {
                result |= CanOrganisationBeDeletedFlags.HasReturns;
            }

            if (await organisationDataAccess.HasActiveUsers(organisationId))
            {
                result |= CanOrganisationBeDeletedFlags.HasActiveUsers;
            }

            if (await organisationDataAccess.HasScheme(organisationId))
            {
                result |= CanOrganisationBeDeletedFlags.HasScheme;
            }

            if (await organisationDataAccess.HasFacility(organisationId, FacilityType.Aatf))
            {
                result |= CanOrganisationBeDeletedFlags.HasAatf;
            }

            if (await organisationDataAccess.HasFacility(organisationId, FacilityType.Ae))
            {
                result |= CanOrganisationBeDeletedFlags.HasAe;
            }

            return result;
        }
    }
}
