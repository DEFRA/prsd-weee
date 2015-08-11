namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Domain.Organisation;
    using Domain.Scheme;
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
                        Enum.Parse(typeof(SchemeStatus), source.SchemeStatus.Value.ToString()),
                SchemeName = source.SchemeName,
                ApprovalName = source.ApprovalNumber,
                IbisCustomerReference = source.IbisCustomerReference,
                ObligationType = source.ObligationType != null ? (ObligationType)Enum.Parse(typeof(ObligationType), source.ObligationType.Value.ToString()) : ObligationType.NotSet,
                CompetentAuthorityId = source.CompetentAuthorityId
            };
        }
    }
}
