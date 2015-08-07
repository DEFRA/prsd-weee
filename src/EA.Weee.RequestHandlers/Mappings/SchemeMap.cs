namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.PCS;
    using Domain.Organisation;
    using Domain.PCS;
    using Prsd.Core.Mapper;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class SchemeMap : IMap<Scheme, SchemeData>
    {
        public SchemeData Map(Scheme source)
        {
            return new SchemeData
            {
                Id = source.Id,
                Name = source.Organisation.OrganisationType.Value == OrganisationType.RegisteredCompany.Value
                    ? source.Organisation.Name
                    : source.Organisation.TradingName,
                SchemeStatus =
                    (SchemeStatus)
                        Enum.Parse(typeof(SchemeStatus), source.SchemeStatus.Value.ToString())
            };
        }
    }
}
