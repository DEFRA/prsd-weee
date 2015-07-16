namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.PCS;
    using Domain.Producer;
    using Prsd.Core.Mediator;
    using Requests.PCS.MemberRegistration;
    using XmlValidation;

    internal class ValidateXmlFileHandler : IRequestHandler<ValidateXmlFile, Guid>
    {
        private readonly WeeeContext context;

        private readonly IXmlValidator xmlValidator;

        public ValidateXmlFileHandler(WeeeContext context, IXmlValidator xmlValidator)
        {
            this.context = context;
            this.xmlValidator = xmlValidator;
        }

        public async Task<Guid> HandleAsync(ValidateXmlFile message)
        {
            var errors = xmlValidator.Validate(message);

            var memberUploadErrors = errors as IList<MemberUploadError> ?? errors.ToList();

            var scheme = await context.Schemes.SingleAsync(c => c.OrganisationId == message.OrganisationId);
            var upload = new MemberUpload(message.OrganisationId, message.Data, memberUploadErrors.ToList(), scheme.Id);

            //Build producers domain object if there are no errors during validation of xml file.
            if (!memberUploadErrors.Any())
            {
                var producers = new List<Producer>();
                try
                {
                    producers = await BuildProducerDataFromXml.SetProducerData(scheme.Id, upload, context, message.Data);
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Error when extracting producer data from XML.");
                }
                if (producers.Count > 0)
                {
                    upload.SetProducers(producers);
                    scheme.SetProducers(producers);
                    context.Producers.AddRange(producers);
                }
            }

            context.MemberUploads.Add(upload);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return upload.Id;
        }
    }
}
