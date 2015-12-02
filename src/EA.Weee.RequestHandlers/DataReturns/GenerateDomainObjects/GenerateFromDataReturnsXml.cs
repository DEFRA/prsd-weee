namespace EA.Weee.RequestHandlers.DataReturns.GenerateDomainObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Prsd.Core;
    using Requests.DataReturns;
    using Xml.Converter;

    public class GenerateFromDataReturnsXML : IGenerateFromDataReturnsXML
    {
        private readonly IXmlConverter xmlConverter;

        public GenerateFromDataReturnsXML(IXmlConverter xmlConverter)
        {
            this.xmlConverter = xmlConverter;
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
                var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile.Data));
                return new DataReturnsUpload(xml, errors, int.Parse(deserializedXml.complianceYear), scheme, messageXmlFile.FileName);
            }
        }
    }
}
