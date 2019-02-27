namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Error;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Xml.MemberRegistration;
    using Interfaces;
    using Prsd.Core.Mediator;
    using Requests.Scheme.MemberRegistration;
    using Xml.Converter;
    using ErrorLevel = Domain.Error.ErrorLevel;

    internal class ProcessXMLFileHandler : IRequestHandler<ProcessXmlFile, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromXml generateFromXml;
        private readonly IXMLChargeBandCalculator xmlChargeBandCalculator;
        private readonly IProducerSubmissionDataAccess producerSubmissionDataAccess;

        public ProcessXMLFileHandler(WeeeContext context, IWeeeAuthorization authorization, 
            IXMLValidator xmlValidator, IGenerateFromXml generateFromXml, IXmlConverter xmlConverter, 
            IXMLChargeBandCalculator xmlChargeBandCalculator, IProducerSubmissionDataAccess producerSubmissionDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.generateFromXml = generateFromXml;
            this.producerSubmissionDataAccess = producerSubmissionDataAccess;
        }

        public async Task<Guid> HandleAsync(ProcessXmlFile message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            // record XML processing start time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var errors = await xmlValidator.Validate(message);

            List<MemberUploadError> memberUploadErrors = errors as List<MemberUploadError> ?? errors.ToList();
            bool containsSchemaErrors = memberUploadErrors.Any(e => e.ErrorType == UploadErrorType.Schema);
            bool containsErrorOrFatal = memberUploadErrors.Any(e => (e.ErrorLevel == ErrorLevel.Error || e.ErrorLevel == ErrorLevel.Fatal));

            TotalChargeCalculator totalChargeCalculator = new TotalChargeCalculator(xmlChargeBandCalculator);
            Dictionary<string, ProducerCharge> producerCharges = null;

            decimal? totalChargesCalculated = 0;
            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            //var complianceYear = await context.RegisteredProducers.FirstOrDefaultAsync(c => c.Scheme.Id == scheme.Id);

            var deserializedXml = xmlConverter.Deserialize<schemeType>(xmlConverter.Convert(message.Data));

            var hasAnnualCharge = false;

            if ((!containsSchemaErrors && !containsErrorOrFatal))
            {
                producerCharges = totalChargeCalculator.TotalCalculatedCharges(message, scheme, int.Parse(deserializedXml.complianceYear), ref hasAnnualCharge, ref totalChargesCalculated);
                if (xmlChargeBandCalculator.ErrorsAndWarnings.Any(e => e.ErrorLevel == ErrorLevel.Error)
                    && memberUploadErrors.All(e => e.ErrorLevel != ErrorLevel.Error))
                {
                    throw new ApplicationException(String.Format(
                        "Upload for Organisation '{0}' has no validation errors, but does have producer charge calculation errors which are not currently being enforced",
                        message.OrganisationId));
                }
            }

            var totalCharges = totalChargesCalculated ?? 0;
            var upload = generateFromXml.GenerateMemberUpload(message, memberUploadErrors, totalCharges, scheme, hasAnnualCharge);
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
            producerSubmissionDataAccess.AddRange(producers);

            await context.SaveChangesAsync();
            return upload.Id;
        }
    }
}
