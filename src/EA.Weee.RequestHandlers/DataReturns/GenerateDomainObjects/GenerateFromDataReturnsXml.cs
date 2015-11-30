namespace EA.Weee.RequestHandlers.DataReturns.GenerateDomainObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.Scheme;
    using Requests.DataReturns;
    using Xml.Converter;

    public class GenerateFromDataReturnsXML : IGenerateFromDataReturnsXML
    {
        private readonly IXmlConverter xmlConverter;

        public GenerateFromDataReturnsXML(IXmlConverter xmlConverter)
        {
            this.xmlConverter = xmlConverter;
        }

        public DataReturnsUpload GenerateDataReturnsUpload(ProcessDataReturnsXMLFile messageXmlFile, List<DataReturnsUploadError> errors, Guid schemeId)
        {
            if (errors != null && errors.Any(e => e.ErrorType == UploadErrorType.Schema))
            {
                return new DataReturnsUpload(xmlConverter.XmlToUtf8String(messageXmlFile.Data), errors, null, schemeId, messageXmlFile.FileName);
            }
            else
            {
                var xml = xmlConverter.XmlToUtf8String(messageXmlFile.Data);
                var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile.Data));
                return new DataReturnsUpload(xml, errors, int.Parse(deserializedXml.complianceYear), schemeId, messageXmlFile.FileName);
            }
        }
    }
}
