﻿namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using Prsd.Core;
    using Requests.DataReturns;
    using System.Linq;
    using Xml;
    using Xml.Converter;
    using Xml.Deserialization;
    using XmlValidation.Errors;
    using XmlValidation.SchemaValidation;

    public class GenerateFromDataReturnXml : IGenerateFromDataReturnXml
    {
        private readonly ISchemaValidator schemaValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IXmlErrorTranslator errorTranslator;

        public GenerateFromDataReturnXml(ISchemaValidator schemaValidator, IXmlConverter xmlConverter, IXmlErrorTranslator errorTranslator)
        {
            this.schemaValidator = schemaValidator;
            this.errorTranslator = errorTranslator;
            this.xmlConverter = xmlConverter;
        }

        public GenerateFromDataReturnXmlResult<T> GenerateDataReturns<T>(ProcessDataReturnXmlFile message) where T : class
        {
            Guard.ArgumentNotNull(() => message, message);

            T deserialisedType = null;

            string schemaVersion = "3.36";

            // Validate against the schema
            var validationErrors = schemaValidator
                .Validate(message.Data, @"EA.Weee.Xml.DataReturns.v3schema.xsd", XmlNamespace.DataReturns, schemaVersion)
                .ToList();

            if (!validationErrors.Any())
            {
                try
                {
                    deserialisedType = xmlConverter.Deserialize<T>(xmlConverter.Convert(message.Data));
                }
                catch (XmlDeserializationFailureException e)
                {
                    // Couldn't deserialise - can't go any further, add an error and bail out here
                    var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    var friendlyMessage = errorTranslator.MakeFriendlyErrorMessage(exceptionMessage, schemaVersion);
                    validationErrors.Add(new XmlValidationError(Core.Shared.ErrorLevel.Error, XmlErrorType.Schema, friendlyMessage));
                }
            }

            return new GenerateFromDataReturnXmlResult<T>(xmlConverter.XmlToUtf8String(message.Data), deserialisedType, validationErrors);
        }
    }
}
