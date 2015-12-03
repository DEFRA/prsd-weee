namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Requests.DataReturns;

    public interface IGenerateFromDataReturnsXml
    { 
        DataReturnsUpload GenerateDataReturnsUpload(ProcessDataReturnsXMLFile messageXmlFile, List<DataReturnsUploadError> errors, Scheme scheme);
    }
}
