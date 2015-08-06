namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.PCS;
    using Domain.Organisation;
    using Domain.PCS;
    using Prsd.Core.Mapper;
    using PcsStatus = Core.Shared.PcsStatus;

    public class PcsMap : IMap<Scheme, PcsData>
    {
        public PcsData Map(Scheme source)
        {
            return new PcsData
            {
                Id = source.Id,
                Name = source.Organisation.OrganisationType.Value == OrganisationType.RegisteredCompany.Value
                    ? source.Organisation.Name
                    : source.Organisation.TradingName,
                PcsStatus =
                    (PcsStatus)
                        Enum.Parse(typeof(PcsStatus), source.PcsStatus.Value.ToString())
            };
        }
    }
}
