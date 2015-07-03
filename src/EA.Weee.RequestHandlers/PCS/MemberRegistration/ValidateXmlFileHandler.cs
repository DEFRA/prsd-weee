namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Requests.PCS.MemberRegistration;

    internal class ValidateXmlFileHandler : IRequestHandler<ValidateXmlFile, Guid>
    {
        private const string SchemaLocation = @"v3schema.xsd";
        
        private readonly WeeeContext context;

        public ValidateXmlFileHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(ValidateXmlFile message)
        {
            var errors = new List<MemberUploadError>();

            try
            {
                var xmlDocument = XDocument.Parse(message.Data, LoadOptions.SetLineInfo);
                var schemas = new XmlSchemaSet();
                var absoluteSchemaLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), SchemaLocation);
                schemas.Add("http://www.environment-agency.gov.uk/WEEE/XMLSchema", absoluteSchemaLocation);

                xmlDocument.Validate(
                    schemas,
                    (sender, args) =>
                        {
                            var asXElement = sender as XElement;
                            errors.Add(
                                asXElement != null
                                    ? new MemberUploadError(ErrorLevel.Error, MakeHumanReadableErrorMessage(asXElement, args))
                                    : new MemberUploadError(ErrorLevel.Error, args.Exception.Message));
                        },
                    false);
            }
            catch (XmlException ex)
            {
                errors.Add(new MemberUploadError(ErrorLevel.Error, ex.Message));
            }
            
            MemberUpload upload = new MemberUpload(message.OrganisationId, message.Data, errors);

            context.MemberUploads.Add(upload);

            await context.SaveChangesAsync();

            return upload.Id;
        }

        private string MakeHumanReadableErrorMessage(XElement sender, ValidationEventArgs args)
        {
            const string GeneralConstraintFailurePattern =
                @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The ([^']*) constraint failed.$";

            const string LengthConstraintFailurePattern =
                @"^The '[^']*' element is invalid - The value '[^']*' is invalid according to its datatype '[^']*' - The actual length is (less|greater) than the (MinLength|MaxLength) value.$";

            const string InvalidChildElementPattern =
                @"^The element '([^']*)' in namespace '[^']*' has invalid child element '([^']*)' in namespace '[^']*'. List of possible elements expected: '[^']*' in namespace '[^']*'.$";

            if (Regex.IsMatch(args.Exception.Message, GeneralConstraintFailurePattern))
            {
                var constraintWhichFailed = Regex.Match(args.Exception.Message, GeneralConstraintFailurePattern).Groups[1].ToString();

                switch (constraintWhichFailed)
                {
                    case "MinInclusive":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} is too low.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                    case "MaxInclusive":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} is too high.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                    case "Pattern":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} doesn't match the required format.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                    case "Enumeration":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} isn't one of the accepted values.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                }
            }

            if (Regex.IsMatch(args.Exception.Message, LengthConstraintFailurePattern))
            {
                var lengthConstraintWhichFailed = Regex.Match(args.Exception.Message, LengthConstraintFailurePattern).Groups[2].ToString();

                switch (lengthConstraintWhichFailed)
                {
                    case "MinLength":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} is too short.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                    case "MaxLength":
                        return string.Format("The value '{0}' supplied for type '{1}' on line {2} is too long.", sender.Value, sender.Name.LocalName, args.Exception.LineNumber);
                }
            }

            if (Regex.IsMatch(args.Exception.Message, InvalidChildElementPattern))
            {
                return "Invalid child element oh no! " + args.Exception.Message;
            }

            return args.Exception.Message;
        }
    }
}
