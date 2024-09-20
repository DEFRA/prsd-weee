namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using Domain.Producer;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal class AddSmallProducerSubmissionHandler : IRequestHandler<AddSmallProducerSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IGenerateFromXmlDataAccess generateFromXmlDataAccess;
        private readonly ISystemDataDataAccess systemDataAccess;

        public AddSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext, 
            IGenerateFromXmlDataAccess generateFromXmlDataAccess, ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.generateFromXmlDataAccess = generateFromXmlDataAccess;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<Guid> HandleAsync(AddSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var currentYearSubmission =
                directRegistrant.DirectProducerSubmissions.Where(d => d.ComplianceYear == systemDateTime.Year);

            if (currentYearSubmission.Any())
            {
                throw new InvalidOperationException($"Producer submission for compliance year {systemDateTime.Year} already exists");
            }

            var existingProducer = directRegistrant.DirectProducerSubmissions.Where(c => c.RegisteredProducer.ComplianceYear != systemDateTime.Year)
                .Select(r => r.RegisteredProducer).FirstOrDefault();

            string producerRegistrationNumber;

            if (existingProducer != null)
            {
                producerRegistrationNumber = existingProducer.ProducerRegistrationNumber;
            }
            else
            {
                var generatedPrn = await generateFromXmlDataAccess.ComputePrns(1);

                producerRegistrationNumber = generatedPrn.Dequeue();

                var exists = await generateFromXmlDataAccess.ProducerRegistrationExists(producerRegistrationNumber);

                if (exists)
                {
                    throw new InvalidOperationException($"Producer number {producerRegistrationNumber} already exists");
                }
            }

            var registeredProducer = new RegisteredProducer(producerRegistrationNumber, systemDateTime.Year);

            var directRegistrantSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, systemDateTime.Year);
            var directProducerSubmissionHistory = new DirectProducerSubmissionHistory(directRegistrantSubmission);

            await genericDataAccess.Add(directProducerSubmissionHistory);

            directRegistrantSubmission.SetCurrentSubmission(directProducerSubmissionHistory);

            await weeeContext.SaveChangesAsync();

            return directRegistrantSubmission.Id;
        }
    }
}
