namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;

    public class PanAreaMap : IMap<PanArea, PanAreaData>
    {
        public PanAreaData Map(PanArea source)
        {
            return new PanAreaData
            {
                Name = source.Name,
                Id = source.Id,
                CompetentAuthorityId = source.CompetentAuthorityId
            };
        }
    }
}
