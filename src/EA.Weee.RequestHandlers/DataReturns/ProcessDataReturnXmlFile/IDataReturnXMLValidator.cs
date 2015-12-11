namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Requests.DataReturns;

    public interface IDataReturnXmlValidator
    {
        IEnumerable<DataReturnUploadError> Validate(ProcessDataReturnXmlFile message);
    }
}
