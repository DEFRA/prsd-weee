namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Domain.Scheme;
    using Requests.DataReturns;

    public interface IGenerateFromDataReturnXml
    {
        DataReturnUpload GenerateDataReturnsUpload(ProcessDataReturnXmlFile messageXmlFile, List<DataReturnUploadError> errors, Scheme scheme);
    }
}
