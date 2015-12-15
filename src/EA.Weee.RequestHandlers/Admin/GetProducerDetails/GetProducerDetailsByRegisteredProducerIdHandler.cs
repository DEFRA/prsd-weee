namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Security;

    public class GetProducerDetailsByRegisteredProducerIdHandler : IRequestHandler<Requests.Admin.GetProducerDetailsByRegisteredProducerId, ProducerDetailsScheme>
    {
        private readonly IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetProducerDetailsByRegisteredProducerIdHandler(IGetProducerDetailsByRegisteredProducerIdDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<ProducerDetailsScheme> HandleAsync(Requests.Admin.GetProducerDetailsByRegisteredProducerId request)
        {
            authorization.EnsureCanAccessInternalArea();

            var producer = await dataAccess.Fetch(request.RegisteredProducerId);

            if (producer == null)
            {
                string message = string.Format(
                    "No producer has been registered with the id \"{0}\".",
                    request.RegisteredProducerId);

                throw new Exception(message);
            }

            return new ProducerDetailsScheme
            {
                SchemeName = producer.Scheme.SchemeName,
                ComplianceYear = producer.ComplianceYear,
                ProducerName = producer.CurrentSubmission.OrganisationName,
                Prn = producer.ProducerRegistrationNumber
            };
        }
    }
}
