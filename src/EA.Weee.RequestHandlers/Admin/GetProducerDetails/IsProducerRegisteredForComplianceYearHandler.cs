namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.Repositories;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class IsProducerRegisteredForComplianceYearHandler : IRequestHandler<IsProducerRegisteredForComplianceYear, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerRepository registeredProducerRepository;

        public IsProducerRegisteredForComplianceYearHandler(IWeeeAuthorization authorization, IRegisteredProducerRepository registeredProducerRepository)
        {
            this.authorization = authorization;
            this.registeredProducerRepository = registeredProducerRepository;
        }

        public async Task<bool> HandleAsync(IsProducerRegisteredForComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producerRegistrations = await registeredProducerRepository.GetProducerRegistrations(request.RegistrationNumber, request.ComplianceYear);

            return producerRegistrations.Any();
        }
    }
}
