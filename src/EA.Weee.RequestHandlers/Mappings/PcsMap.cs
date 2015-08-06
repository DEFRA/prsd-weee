namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.PCS;
    using Core.Shared;
    using Domain.PCS;
    using Prsd.Core.Mapper;

    public class PcsMap : IMap<Scheme, PcsData>
    {
        public PcsData Map(Scheme source)
        {
            return new PcsData
            {
                Name = source.Organisation.Name,
                PcsStatus =
                    (PcsStatus)
                        Enum.Parse(typeof(PcsStatus), source.PcsStatus.Value.ToString())
            };
        }
    }
}
