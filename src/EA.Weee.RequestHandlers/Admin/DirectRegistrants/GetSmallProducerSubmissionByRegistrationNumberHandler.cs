namespace EA.Weee.RequestHandlers.Admin.DirectRegistrants
{
    using Core.DirectRegistrant;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using Security;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionByRegistrationNumberHandler : IRequestHandler<GetSmallProducerSubmissionByRegistrationNumber, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;

        public GetSmallProducerSubmissionByRegistrationNumberHandler(
            IWeeeAuthorization authorization,
            IRegisteredProducerDataAccess registeredProducerDataAccess,
            ISmallProducerSubmissionService smallProducerSubmissionService)
        {
            this.authorization = authorization;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
            this.smallProducerSubmissionService = smallProducerSubmissionService;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmissionByRegistrationNumber request)
        {
            authorization.EnsureCanAccessInternalArea();
            var directRegistrant = await registeredProducerDataAccess.GetDirectRegistrantByRegistration(request.RegistrationNumber);

            return await smallProducerSubmissionService.GetSmallProducerSubmissionData(directRegistrant, true);
        }
    }
}
