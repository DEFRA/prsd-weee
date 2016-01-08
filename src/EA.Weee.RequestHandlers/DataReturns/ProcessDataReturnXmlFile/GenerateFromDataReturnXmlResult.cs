namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using XmlValidation.Errors;

    public class GenerateFromDataReturnXmlResult<T> where T : class
    {
        public string XmlString { get; private set; }

        public T DeserialisedType { get; private set; }

        public List<XmlValidationError> SchemaErrors { get; private set; }

        public GenerateFromDataReturnXmlResult(string xmlString, T deserialisedType, List<XmlValidationError> schemaErrors)
        {
            XmlString = xmlString;
            DeserialisedType = deserialisedType;
            SchemaErrors = schemaErrors;
        }
    }
}
