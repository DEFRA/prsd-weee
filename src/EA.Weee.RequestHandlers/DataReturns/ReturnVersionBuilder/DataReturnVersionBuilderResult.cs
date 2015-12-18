namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.DataReturns;

    public class DataReturnVersionBuilderResult
    {
        public DataReturnVersion DataReturnVersion { get; }

        public List<ErrorData> ErrorData { get; }

        public DataReturnVersionBuilderResult(DataReturnVersion dataReturnVersion, List<ErrorData> errorData)
        {
            DataReturnVersion = dataReturnVersion;
            ErrorData = errorData;
        }
    }
}
