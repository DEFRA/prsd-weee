namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.DataReturns;

    public class DataReturnVersionBuilderResult
    {
        public DataReturnVersion DataReturnVersion { get; private set; }

        public List<ErrorData> ErrorData { get; private set; }

        public DataReturnVersionBuilderResult(DataReturnVersion dataReturnVersion, List<ErrorData> errorData)
        {
            DataReturnVersion = dataReturnVersion;
            ErrorData = errorData;
        }
    }
}
