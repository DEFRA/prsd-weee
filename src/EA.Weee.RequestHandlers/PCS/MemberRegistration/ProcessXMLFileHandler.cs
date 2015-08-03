namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.PCS;
    using GenerateProducerObjects;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;
    using XmlValidation;

    internal class ProcessXMLFileHandler : IRequestHandler<ProcessXMLFile, Guid>
    {
        private readonly WeeeContext context;
        private readonly IXmlValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromXml generateFromXml;

        private readonly IXmlChargeBandCalculator xmlChargeBandCalculator;

        public ProcessXMLFileHandler(WeeeContext context, IXmlValidator xmlValidator, IGenerateFromXml generateFromXml, IXmlConverter xmlConverter, IXmlChargeBandCalculator xmlChargeBandCalculator)
        {
            this.context = context;
            this.xmlValidator = xmlValidator;
            this.xmlConverter = xmlConverter;
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
            this.generateFromXml = generateFromXml;
        }

        public async Task<Guid> HandleAsync(ProcessXMLFile message)
        {
            var errors = xmlValidator.Validate(message);

            var memberUploadErrors = errors as IList<MemberUploadError> ?? errors.ToList();

            Hashtable producerCharges = new Hashtable();

            if (!errors.Any(e => e.ErrorType == MemberUploadErrorType.Schema))
            {
                producerCharges = xmlChargeBandCalculator.Calculate(message);

                if (xmlChargeBandCalculator.ErrorsAndWarnings != null && xmlChargeBandCalculator.ErrorsAndWarnings.Count > 0)
                {
                    ((List<MemberUploadError>)memberUploadErrors).AddRange(xmlChargeBandCalculator.ErrorsAndWarnings);
                }
            }

            decimal totalCharges = 0;
            foreach (DictionaryEntry producerCharge in producerCharges)
            {
                totalCharges = totalCharges + ((ProducerCharge)producerCharge.Value).ChargeAmount;
            }

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            var upload = new MemberUpload(message.OrganisationId, xmlConverter.XmlToUtf8String(message), memberUploadErrors.ToList(), totalCharges, scheme.Id);

            //Build producers domain object if there are no errors during validation of xml file.
            if (!memberUploadErrors.Any())
            {
                var producers = await generateFromXml.Generate(message, upload, producerCharges);
                context.MemberUploads.Add(upload);
                context.Producers.AddRange(producers);
            }
            else
            {
                context.MemberUploads.Add(upload);
            }
            await context.SaveChangesAsync();
            return upload.Id;
        }
    }
}
