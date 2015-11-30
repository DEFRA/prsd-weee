namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.DataReturns;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class DataReturnsUploadMap : IMap<DataReturnsUpload, DataReturnsUploadData>
    {
        public DataReturnsUploadData Map(DataReturnsUpload source)
        {
            return new DataReturnsUploadData
            {
                Id = source.Id,
                ComplianceYear = source.ComplianceYear,
                SchemeId = source.SchemeId,
                IsSubmitted = source.IsSubmitted
            };
        }
    }
}
