namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.Lookup;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;

    public class PanAreaMap : IMap<PanArea, PanAreaData>
    {
        public PanAreaData Map(PanArea source)
        {
            if (source != null)
            {
                return new PanAreaData
                {
                    Name = source.Name,
                    Id = source.Id,
                    CompetentAuthorityId = source.CompetentAuthorityId
                };
            }

            return new PanAreaData();
        }
    }
}
