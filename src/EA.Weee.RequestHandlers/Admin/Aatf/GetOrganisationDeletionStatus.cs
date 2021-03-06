﻿namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;

    public class GetOrganisationDeletionStatus : IGetOrganisationDeletionStatus
    {
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetOrganisationDeletionStatus(IOrganisationDataAccess organisationDataAccess)
        {
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<CanOrganisationBeDeletedFlags> Validate(Guid organisationId, int year, FacilityType facilityType)
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

            if (await organisationDataAccess.HasMultipleOfEntityFacility(organisationId, facilityType))
            {
                result |= CanOrganisationBeDeletedFlags.HasMultipleOfFacility;
            }

            return result;
        }
    }
}
