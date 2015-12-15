namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
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
    using Xml.DataReturns;
    using Xml.Deserialization;

    public class GenerateFromDataReturnXml : IGenerateFromDataReturnXml
    {
        private readonly IXmlConverter xmlConverter;
        private readonly IDeserializer deserializer;       

        public GenerateFromDataReturnXml(IXmlConverter xmlConverter, IDeserializer deserializer)
        {
            this.xmlConverter = xmlConverter;
            this.deserializer = deserializer;
        }

        public DataReturnUpload GenerateDataReturnUpload(
            ProcessDataReturnXmlFile messageXmlFile,
            List<DataReturnUploadError> errors,
            Scheme scheme)
        {
            Guard.ArgumentNotNull(() => messageXmlFile, messageXmlFile);
            Guard.ArgumentNotNull(() => errors, errors);
            Guard.ArgumentNotNull(() => scheme, scheme);

            var xml = xmlConverter.XmlToUtf8String(messageXmlFile.Data);
           
            if (errors != null && errors.Any(e => e.ErrorType == UploadErrorType.Schema))
            {                
                return new DataReturnUpload(scheme, xml, errors, messageXmlFile.FileName, null, null); 
            }
            else
            {
                SchemeReturn deserializedXml = deserializer.Deserialize<SchemeReturn>(xmlConverter.Convert(messageXmlFile.Data));
                int complianceYear = int.Parse(deserializedXml.ComplianceYear);
                int quarter = Convert.ToInt32(deserializedXml.ReturnPeriod);               
                return new DataReturnUpload(scheme, xml, errors, messageXmlFile.FileName, complianceYear, quarter);
            }
        }       
    }
}
