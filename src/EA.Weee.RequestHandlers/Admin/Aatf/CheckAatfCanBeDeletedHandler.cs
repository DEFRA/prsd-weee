namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Threading.Tasks;
    using AatfReturn.Internal;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
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

            var aatfDeletion = await getAatfDeletionStatus.Validate(message.AatfId);

            var aatf = await aatfDataAccess.GetDetails(message.AatfId);

            var organisationDeletion = await getOrganisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear);

            return new AatfDeletionData(organisationDeletion, aatfDeletion);
        }
    }
}
