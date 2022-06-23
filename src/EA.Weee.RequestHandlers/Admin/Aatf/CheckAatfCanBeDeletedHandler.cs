namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;

    public class CheckAatfCanBeDeletedHandler : IRequestHandler<CheckAatfCanBeDeleted, AatfDeletionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfDeletionStatus getAatfDeletionStatus;
        private readonly IGetOrganisationDeletionStatus getOrganisationDeletionStatus;
        private readonly IAatfDataAccess aatfDataAccess;

        public CheckAatfCanBeDeletedHandler(IWeeeAuthorization authorization,
            IGetAatfDeletionStatus getAatfDeletionStatus,
            IGetOrganisationDeletionStatus getOrganisationDeletionStatus,
            IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.getAatfDeletionStatus = getAatfDeletionStatus;
            this.getOrganisationDeletionStatus = getOrganisationDeletionStatus;
            this.aatfDataAccess = aatfDataAccess;
        }

        public async Task<AatfDeletionData> HandleAsync(CheckAatfCanBeDeleted message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var aatf = await aatfDataAccess.GetDetails(message.AatfId);

            var aatfDeletion = await getAatfDeletionStatus.Validate(aatf.Id);

            var organisationDeletion = await getOrganisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType);

            return new AatfDeletionData(organisationDeletion, aatfDeletion);
        }
    }
}
