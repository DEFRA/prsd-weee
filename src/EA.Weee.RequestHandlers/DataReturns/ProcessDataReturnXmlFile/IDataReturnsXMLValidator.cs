namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Requests.DataReturns;

    public interface IDataReturnsXmlValidator
    {
        IEnumerable<DataReturnsUploadError> Validate(ProcessDataReturnsXMLFile message);
    }
}
