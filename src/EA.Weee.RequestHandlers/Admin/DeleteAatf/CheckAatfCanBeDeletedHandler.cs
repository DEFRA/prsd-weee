namespace EA.Weee.RequestHandlers.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using DeleteValidation;

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
