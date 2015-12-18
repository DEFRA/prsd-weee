namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using XmlValidation.Errors;

    public class GenerateFromDataReturnXmlResult<T> where T : class
    {
        public string XmlString { get; }

        public T DeserialisedType { get; }

        public List<XmlValidationError> SchemaErrors { get; }

        public GenerateFromDataReturnXmlResult(string xmlString, T deserialisedType, List<XmlValidationError> schemaErrors)
        {
            XmlString = xmlString;
            DeserialisedType = deserialisedType;
            SchemaErrors = schemaErrors;
        }
    }
}
