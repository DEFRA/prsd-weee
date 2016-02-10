namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeDataAvailabilityMap : IMap<SchemeDataAvailability, Core.Scheme.SchemeDataAvailability>
    {
        private readonly IMapper mapper;

        public SchemeDataAvailabilityMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Core.Scheme.SchemeDataAvailability Map(SchemeDataAvailability source)
        {
            return new Core.Scheme.SchemeDataAvailability
            {
                AnnualDataAvailibilities = source.DownloadsByYears
                    .Select(s => mapper.Map<SchemeAnnualDataAvailability, Core.Scheme.SchemeAnnualDataAvailability>(s))
                    .ToList()
            };
        }
    }
}
