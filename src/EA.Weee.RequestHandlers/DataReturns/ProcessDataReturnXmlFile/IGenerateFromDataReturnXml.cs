namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Requests.DataReturns;

    public interface IGenerateFromDataReturnXML
    {
        DataReturnUpload GenerateDataReturnsUpload(ProcessDataReturnXMLFile messageXmlFile, List<DataReturnUploadError> errors, Scheme scheme);
    }
}
