namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using ObligationType = Core.Shared.ObligationType;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class SchemeMap : IMap<Scheme, SchemeData>
    {
        private readonly IMap<UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;

        public SchemeMap(IMap<UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
        }

        public SchemeData Map(Scheme source)
        {
            return new SchemeData
            {
                Id = source.Id,
                OrganisationId = source.OrganisationId,
                Name = source.Organisation.OrganisationType.Value == OrganisationType.RegisteredCompany.Value
                    ? source.Organisation.Name
                    : source.Organisation.TradingName,
                SchemeStatus =
                    (SchemeStatus)
                        Enum.Parse(typeof(SchemeStatus), source.SchemeStatus.Value.ToString()),
                SchemeName = source.SchemeName,
                ApprovalName = source.ApprovalNumber,
                IbisCustomerReference = source.IbisCustomerReference,
                ObligationType = source.ObligationType != null ? (ObligationType)Enum.Parse(typeof(ObligationType), source.ObligationType.Value.ToString()) : (ObligationType?)null,
                CompetentAuthorityId = source.CompetentAuthorityId,
                CompetentAuthority = source.CompetentAuthority != null
                    ? competentAuthorityMap.Map(source.CompetentAuthority)
                    : null,
            };
        }
    }
}
