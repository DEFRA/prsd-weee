namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Organisations;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using System;
    using OrganisationStatus = Core.Shared.OrganisationStatus;

    public class OrganisationNameStatusMap : IMap<Organisation, OrganisationNameStatus>
    {
        public OrganisationNameStatus Map(Organisation source)
        {
            return new OrganisationNameStatus
            {
                Name = source.OrganisationName,
                Status =
                    (OrganisationStatus)
                        Enum.Parse(typeof(OrganisationStatus), source.OrganisationStatus.Value.ToString()),
            };
        }
    }
}
