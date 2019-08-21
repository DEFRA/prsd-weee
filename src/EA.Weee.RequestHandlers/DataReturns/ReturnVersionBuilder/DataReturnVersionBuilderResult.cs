namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using Core.Shared;
    using Domain.DataReturns;
    using System.Collections.Generic;

    public class DataReturnVersionBuilderResult
    {
        public DataReturnVersion DataReturnVersion { get; private set; }

        public List<ErrorData> ErrorData { get; private set; }

        public DataReturnVersionBuilderResult()
        {
            ErrorData = new List<ErrorData>();
        }

        public DataReturnVersionBuilderResult(DataReturnVersion dataReturnVersion, List<ErrorData> errorData)
        {
            DataReturnVersion = dataReturnVersion;
            ErrorData = errorData;
        }
    }
}
