namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core;
    using Requests.DataReturns;
    using Xml.Converter;
    using Xml.DataReturns;
    using Xml.Deserialization;

    public class GenerateFromDataReturnsXml : IGenerateFromDataReturnsXml
    {
        private readonly IXmlConverter xmlConverter;
        private readonly IDeserializer deserializer;

        public GenerateFromDataReturnsXml(IXmlConverter xmlConverter, IDeserializer deserializer)
        {
            this.xmlConverter = xmlConverter;
            this.deserializer = deserializer;
        }

        public DataReturnsUpload GenerateDataReturnsUpload(
            ProcessDataReturnsXMLFile messageXmlFile,
            List<DataReturnsUploadError> errors,
            Scheme scheme)
        {
            Guard.ArgumentNotNull(() => messageXmlFile, messageXmlFile);
            Guard.ArgumentNotNull(() => errors, errors);
            Guard.ArgumentNotNull(() => scheme, scheme);

            if (errors != null && errors.Any(e => e.ErrorType == UploadErrorType.Schema))
            {
                return new DataReturnsUpload(xmlConverter.XmlToUtf8String(messageXmlFile.Data), errors, null, scheme, messageXmlFile.FileName);
            }
            else
            {
                var xml = xmlConverter.XmlToUtf8String(messageXmlFile.Data);
                SchemeReturn deserializedXml = deserializer.Deserialize<SchemeReturn>(xmlConverter.Convert(messageXmlFile.Data));
                return new DataReturnsUpload(xml, errors, int.Parse(deserializedXml.ComplianceYear), scheme, messageXmlFile.FileName);
            }
        }
    }
}
