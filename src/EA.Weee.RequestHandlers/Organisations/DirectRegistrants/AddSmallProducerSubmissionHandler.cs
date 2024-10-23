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
        private readonly ISmallProducerDataAccess smallProducerDataAccess;

        public AddSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext, 
            IGenerateFromXmlDataAccess generateFromXmlDataAccess, ISystemDataDataAccess systemDataAccess, ISmallProducerDataAccess smallProducerDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.generateFromXmlDataAccess = generateFromXmlDataAccess;
            this.systemDataAccess = systemDataAccess;
            this.smallProducerDataAccess = smallProducerDataAccess;
        }

        public async Task<Guid> HandleAsync(AddSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var currentYearSubmission =
                await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionByComplianceYear(request.DirectRegistrantId, systemDateTime.Year);

            if (currentYearSubmission != null)
            {
                throw new InvalidOperationException($"Producer submission for compliance year {systemDateTime.Year} already exists");
            }

            var existingProducer = directRegistrant.DirectProducerSubmissions.Select(r => r.RegisteredProducer).FirstOrDefault();

            string producerRegistrationNumber;

            if (!string.IsNullOrWhiteSpace(directRegistrant.ProducerRegistrationNumber))
            {
                producerRegistrationNumber = directRegistrant.ProducerRegistrationNumber;
            }
            else
            {
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
