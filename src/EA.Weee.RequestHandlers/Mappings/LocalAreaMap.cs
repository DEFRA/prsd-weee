namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Domain.Lookup;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LocalAreaMap : IMap<LocalArea, LocalAreaData>
    {
        public LocalAreaData Map(LocalArea source)
        {
            if (source != null)
            {
                return new LocalAreaData
                {
                    Name = source.Name,
                    Id = source.Id,
                    CompetentAuthorityId = source.CompetentAuthorityId
                };
            }

            return null;
        }
    }
}
