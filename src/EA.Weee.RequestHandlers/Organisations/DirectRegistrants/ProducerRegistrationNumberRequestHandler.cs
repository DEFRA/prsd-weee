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
        private readonly GenerateFromXmlDataAccess dataAccess;

        public ProducerRegistrationNumberRequestHandler(GenerateFromXmlDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(ProducerRegistrationNumberRequest request)
        {
            return await dataAccess.SchemeProducerRegistrationExists(request.ProducerRegistrationNumber);
        }
    }
}
