namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.PCS;
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

            var upload = new MemberUpload(message.OrganisationId, message.Data, errors.ToList());

            context.MemberUploads.Add(upload);

            await context.SaveChangesAsync();

            return upload.Id;
        }
    }
}
