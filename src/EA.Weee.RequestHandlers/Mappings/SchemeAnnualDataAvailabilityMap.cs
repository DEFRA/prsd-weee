namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class SchemeAnnualDataAvailabilityMap : IMap<SchemeAnnualDataAvailability, Core.Scheme.SchemeAnnualDataAvailability>
    {
        public Core.Scheme.SchemeAnnualDataAvailability Map(SchemeAnnualDataAvailability source)
        {
            return new Core.Scheme.SchemeAnnualDataAvailability
            {
                IsDataReturnsDownloadAvailable = source.IsDataReturnsDownloadAvailable,
                IsMembersDownloadAvailable = source.IsMembersDownloadAvailable,
                Year = source.Year
            };
        }
    }
}
