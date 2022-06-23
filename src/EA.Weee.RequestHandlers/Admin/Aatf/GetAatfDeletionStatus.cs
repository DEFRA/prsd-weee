namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using Domain.AatfReturn;
    using System;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;

    public class GetAatfDeletionStatus : IGetAatfDeletionStatus
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGetOrganisationDeletionStatus getOrganisationDeletionStatus;

        public GetAatfDeletionStatus(IAatfDataAccess aatfDataAccess,
            IGetOrganisationDeletionStatus getOrganisationDeletionStatus)
        {
            this.aatfDataAccess = aatfDataAccess;
            this.getOrganisationDeletionStatus = getOrganisationDeletionStatus;
        }

        public async Task<CanAatfBeDeletedFlags> Validate(Guid aatfId)
        {
            var result = new CanAatfBeDeletedFlags();

            var aatf = await aatfDataAccess.GetDetails(aatfId);

            var isLatest = await aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId);

            if (!isLatest)
            {
                result |= CanAatfBeDeletedFlags.IsNotLatest;
                return result;
            }

            var hasData = await aatfDataAccess.HasAatfData(aatfId);

            if (hasData)
            {
                result |= CanAatfBeDeletedFlags.HasData;
                return result;
            }

            var organisationDeletionStatus = await getOrganisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType);

            if (organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasReturns) &&
                await aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf))
            {
                result |= CanAatfBeDeletedFlags.CanDelete;
            }
            else if (!organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasReturns) &&
                     await aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf))
            {
                result |= CanAatfBeDeletedFlags.CanDelete;
            }
            else if (!organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasReturns) &&
                     !await aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf))
            {
                result |= CanAatfBeDeletedFlags.CanDelete;

                if (!organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasScheme) && !organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasMultipleOfFacility)
                && (aatf.FacilityType == FacilityType.Aatf && !organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasAe)
                    || (aatf.FacilityType == FacilityType.Ae && !organisationDeletionStatus.HasFlag(CanOrganisationBeDeletedFlags.HasAatf))))
                {
                    result |= CanAatfBeDeletedFlags.CanDeleteOrganisation;
                }
            }

            return result;
        }
    }
}
