namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.DirectRegistrant;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public GetSmallProducerSubmissionHandler(
            IWeeeAuthorization authorization,
            ISmallProducerSubmissionService smallProducerSubmissionService,
            ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.authorization = authorization;
            this.smallProducerSubmissionService = smallProducerSubmissionService;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        {
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await smallProducerDataAccess.GetById(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            return await smallProducerSubmissionService.GetSmallProducerSubmissionData(directRegistrant);
        }
    }
}
