namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Domain.Producer;
    using EA.Weee.DataAccess.DataAccess;
    using Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal abstract class SubmissionRequestHandlerBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        protected SubmissionRequestHandlerBase(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, ISystemDataDataAccess systemDataAccess, ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.systemDataAccess = systemDataAccess;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<DirectProducerSubmission> Get(Guid directRegistrantId)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(directRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var currentYearSubmission =
                await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(directRegistrantId,
                    systemDateTime.Year);

            if (currentYearSubmission == null)
            {
                throw new InvalidOperationException(
                    $"SubmissionRequestHandlerBase current year submission for {systemDateTime.Year} not found");
            }

            return currentYearSubmission;
        }
    }
}
