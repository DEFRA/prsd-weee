namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Domain.Producer;
    using EA.Weee.DataAccess.DataAccess;
    using Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal abstract class EditSubmissionRequestHandlerBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataAccess;

        protected EditSubmissionRequestHandlerBase(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<DirectProducerSubmission> Get(Guid directRegistrantId)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(directRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.First(r => r.ComplianceYear == systemDateTime.Year);

            return currentYearSubmission;
        }
    }
}
