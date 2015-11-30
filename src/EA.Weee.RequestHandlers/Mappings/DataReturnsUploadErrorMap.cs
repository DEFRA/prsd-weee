namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class DataReturnsUploadErrorMap : IMap<DataReturnsUploadError, UploadErrorData>
    {
        public UploadErrorData Map(DataReturnsUploadError source)
        {
            return new UploadErrorData
            {
                ErrorLevel = (ErrorLevel)source.ErrorLevel.Value,
                Description = source.Description
            };
        }
    }
}
