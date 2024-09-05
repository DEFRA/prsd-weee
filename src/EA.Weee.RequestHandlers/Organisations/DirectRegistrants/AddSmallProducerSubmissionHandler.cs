namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers.PrnGeneration;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    internal class AddSmallProducerSubmissionHandler : IRequestHandler<AddSmallProducerSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IGenerateFromXmlDataAccess generateFromXmlDataAccess;

        public AddSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            WeeeContext weeeContext, 
            IGenerateFromXmlDataAccess generateFromXmlDataAccess, IGenerateFromXml generateFromXml)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.weeeContext = weeeContext;
            this.generateFromXmlDataAccess = generateFromXmlDataAccess;
        }

        public async Task<Guid> HandleAsync(AddSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var test = await generateFromXmlDataAccess.ComputePrns(1);

            var prn = test.Dequeue();

            var exists = await generateFromXmlDataAccess.ProducerRegistrationExists(prn);

            if (exists)
            {
                throw new InvalidOperationException($"Producer number {prn} already exists");
            }

            var year = SystemTime.UtcNow.Year;
            var registeredProducer = new RegisteredProducer(prn, SystemTime.UtcNow.Year);

            var directProducerSubmissionHistory = new DirectProducerSubmissionHistory();
            var directRegistrantSubmission = new DirectProducerSubmission(directRegistrant, registeredProducer, year, directProducerSubmissionHistory);

            await genericDataAccess.Add(directRegistrantSubmission);

            await weeeContext.SaveChangesAsync();

            return directRegistrantSubmission.Id;
        }
    }
}
