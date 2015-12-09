namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Interfaces;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Xml.Converter;

    internal class ProcessXMLFileHandler : IRequestHandler<ProcessXMLFile, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromXml generateFromXml;
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;

        public ProcessXMLFileHandler(WeeeContext context, IWeeeAuthorization authorization, 
            IXMLValidator xmlValidator, IGenerateFromXml generateFromXml, IXmlConverter xmlConverter, 
            IXMLChargeBandCalculator xmlChargeBandCalculator)
        {
            this.context = context;
            this.authorization = authorization;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.generateFromXml = generateFromXml;
        }

        public async Task<Guid> HandleAsync(ProcessXMLFile message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            // record XML processing start time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var errors = xmlValidator.Validate(message);

            List<MemberUploadError> memberUploadErrors = errors as List<MemberUploadError> ?? errors.ToList();
            bool containsSchemaErrors = memberUploadErrors.Any(e => e.ErrorType == UploadErrorType.Schema);
            bool containsErrorOrFatal = memberUploadErrors.Any(e => (e.ErrorLevel == ErrorLevel.Error || e.ErrorLevel == ErrorLevel.Fatal));

            //calculate charge band for producers if no schema errors
            Dictionary<string, ProducerCharge> producerCharges = null;
            decimal totalCharges = 0;
            
            if (!containsSchemaErrors)
            {
                producerCharges = ProducerCharges(message, ref totalCharges);
                if (xmlChargeBandCalculator.ErrorsAndWarnings.Any(e => e.ErrorLevel == ErrorLevel.Error)
                    && memberUploadErrors.All(e => e.ErrorLevel != ErrorLevel.Error))
                {
                    throw new ApplicationException(String.Format(
                        "Upload for Organisation '{0}' has no validation errors, but does have producer charge calculation errors which are not currently being enforced",
                        message.OrganisationId));
                }
            }

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            var upload = generateFromXml.GenerateMemberUpload(message, memberUploadErrors, totalCharges, scheme);
            IEnumerable<ProducerSubmission> producers = Enumerable.Empty<ProducerSubmission>();

            //Build producers domain object if there are no errors (schema or business) during validation of xml file.
            if (!containsErrorOrFatal)
            {
                producers = await generateFromXml.GenerateProducers(message, upload, producerCharges);
            }

            // record XML processing end time
            stopwatch.Stop();
            upload.SetProcessTime(stopwatch.Elapsed);

            context.MemberUploads.Add(upload);
            context.ProducerSubmissions.AddRange(producers);

            await context.SaveChangesAsync();
            return upload.Id;
        }

        private Dictionary<string, ProducerCharge> ProducerCharges(ProcessXMLFile message, ref decimal totalCharges)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(message);

            totalCharges = producerCharges
                .Aggregate(totalCharges, (current, producerCharge) => current + producerCharge.Value.Amount);

            return producerCharges;
        }
    }
}
