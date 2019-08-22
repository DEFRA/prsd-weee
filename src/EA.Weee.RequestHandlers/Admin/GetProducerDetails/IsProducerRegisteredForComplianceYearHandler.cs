namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System.Linq;
    using System.Threading.Tasks;

    public class IsProducerRegisteredForComplianceYearHandler : IRequestHandler<IsProducerRegisteredForComplianceYear, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public IsProducerRegisteredForComplianceYearHandler(IWeeeAuthorization authorization, IRegisteredProducerDataAccess registeredProducerDataAccess)
        {
            this.authorization = authorization;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
        }

        public async Task<bool> HandleAsync(IsProducerRegisteredForComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producerRegistrations = await registeredProducerDataAccess.GetProducerRegistrations(request.RegistrationNumber, request.ComplianceYear);

            return producerRegistrations.Any();
        }
    }
}
