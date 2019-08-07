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

        public CheckAatfCanBeDeletedHandler(IWeeeAuthorization authorization, 
            IGetAatfDeletionStatus getAatfDeletionStatus, 
            IGetOrganisationDeletionStatus getOrganisationDeletionStatus)
        {
            this.authorization = authorization;
            this.getAatfDeletionStatus = getAatfDeletionStatus;
            this.getOrganisationDeletionStatus = getOrganisationDeletionStatus;
        }

        public async Task<AatfDeletionData> HandleAsync(CheckAatfCanBeDeleted message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var aatfDeletion = await getAatfDeletionStatus.Validate(message.AatfId);

            // FIX THIS YEAR
            var organisationDeletion = await getOrganisationDeletionStatus.Validate(message.OrganisationId, 2019);

            return new AatfDeletionData(organisationDeletion, aatfDeletion);
        }
    }
}
