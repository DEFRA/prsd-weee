namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeDownloadsByYearsMap : IMap<SchemeDownloadsByYears, Core.Scheme.SchemeDownloadsByYears>
    {
        private readonly IMapper mapper;

        public SchemeDownloadsByYearsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Core.Scheme.SchemeDownloadsByYears Map(SchemeDownloadsByYears source)
        {
            return new Core.Scheme.SchemeDownloadsByYears
            {
                SchemeDownloads = source.DownloadsByYears
                    .Select(s => mapper.Map<SchemeDownloadsByYear, Core.Scheme.SchemeDownloadsByYear>(s))
                    .ToList()
            };
        }
    }
}
