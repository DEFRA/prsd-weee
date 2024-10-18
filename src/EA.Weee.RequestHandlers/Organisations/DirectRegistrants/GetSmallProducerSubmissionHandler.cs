namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.DirectRegistrant;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;

        public GetSmallProducerSubmissionHandler(
            IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess,
            ISmallProducerSubmissionService smallProducerSubmissionService)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.smallProducerSubmissionService = smallProducerSubmissionService;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        {
            authorization.EnsureCanAccessExternalArea();
            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);
            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            return await smallProducerSubmissionService.GetSmallProducerSubmissionData(directRegistrant);
        }
    }
}
