namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeDownloadsByYearMap : IMap<SchemeDownloadsByYear, Core.Scheme.SchemeDownloadsByYear>
    {
        public Core.Scheme.SchemeDownloadsByYear Map(SchemeDownloadsByYear source)
        {
            return new Core.Scheme.SchemeDownloadsByYear
            {
                IsDataReturnsDownloadAvailable = source.IsDataReturnsDownloadAvailable,
                IsMembersDownloadAvailable = source.IsMembersDownloadAvailable,
                Year = source.Year
            };
        }
    }
}
