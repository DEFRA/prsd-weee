namespace EA.Weee.RequestHandlers.DataReturns.XmlValidation
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using Requests.DataReturns;

    public interface IDataReturnsXMLValidator
    {
        IEnumerable<DataReturnsUploadError> Validate(ProcessDataReturnsXMLFile message);
    }
}
