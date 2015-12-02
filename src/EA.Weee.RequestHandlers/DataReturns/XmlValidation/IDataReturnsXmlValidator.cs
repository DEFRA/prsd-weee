namespace EA.Weee.RequestHandlers.DataReturns.XmlValidation
{
    using System.Collections.Generic;
    using Domain.DataReturns;
    using Requests.DataReturns;

    public interface IDataReturnsXMLValidator
    {
        IEnumerable<DataReturnsUploadError> Validate(ProcessDataReturnsXMLFile message);
    }
}
