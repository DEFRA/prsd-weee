namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class ProducerRegistrationNumberRequestHandler : IRequestHandler<ProducerRegistrationNumberRequest, bool>
    {
        private readonly IGenerateFromXmlDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public ProducerRegistrationNumberRequestHandler(IGenerateFromXmlDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(ProducerRegistrationNumberRequest request)
        {
            authorization.EnsureCanAccessExternalArea();

            return await dataAccess.SchemeProducerRegistrationExists(request.ProducerRegistrationNumber);
        }
    }
}
