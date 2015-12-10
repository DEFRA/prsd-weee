namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.DataReturns;
    using Requests.DataReturns;
    using Shared;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xml.DataReturns;
    using Xml.Deserialization;

    public class DataReturnXMLValidator : IDataReturnXMLValidator
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;
        private readonly IDeserializer deserializer;

        public DataReturnXMLValidator(ISchemaValidator schemaValidator, IXmlConverter xmlConverter, IXmlErrorTranslator errorTranslator, IDeserializer deserializer)
        {
            this.schemaValidator = schemaValidator;
            this.errorTranslator = errorTranslator;
            this.xmlConverter = xmlConverter;
            this.deserializer = deserializer;
        }

        public IEnumerable<DataReturnUploadError> Validate(ProcessDataReturnXMLFile message)
        {
            string schemaVersion = "3.2";
            // Validate against the schema
            var errors = schemaValidator.Validate(message.Data, @"EA.Weee.Xml.DataReturns.v3schema.xsd", @"http://www.environment-agency.gov.uk/WEEE/XMLSchema/SchemeReturns", schemaVersion)
                .Select(e => e.ToDataReturnsUploadError())
                .ToList();

            if (errors.Any())
            {
                return errors;
            }
            
            try
            {
                // Validate deserialized XML against business rules
                deserializer.Deserialize<SchemeReturn>(xmlConverter.Convert(message.Data));
            }
            catch (XmlDeserializationFailureException e)
            {
                // Couldn't deserialise - can't go any further, add an error and bail out here
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                var friendlyMessage = errorTranslator.MakeFriendlyErrorMessage(exceptionMessage, schemaVersion);
                errors.Add(new DataReturnUploadError(ErrorLevel.Error, UploadErrorType.Schema, friendlyMessage));

                return errors;
            }

            //TODO : Business validations
            return errors;
        }
    }
}
