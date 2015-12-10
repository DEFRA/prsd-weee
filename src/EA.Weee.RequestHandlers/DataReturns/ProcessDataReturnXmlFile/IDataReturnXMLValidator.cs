namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Requests.DataReturns;

    public interface IDataReturnXMLValidator
    {
        IEnumerable<DataReturnUploadError> Validate(ProcessDataReturnXMLFile message);
    }
}
