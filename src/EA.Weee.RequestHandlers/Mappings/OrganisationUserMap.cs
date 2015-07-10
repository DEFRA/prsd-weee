﻿namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.Organisations;
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Organisations;
    using Requests.Shared;
    using OrganisationUserStatus = Core.Organisations.OrganisationUserStatus;

    public class OrganisationUserMap : IMap<OrganisationUser, OrganisationUserData>
    {
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        public OrganisationUserMap(IMap<Organisation, OrganisationData> organisationMap)
        {
            this.organisationMap = organisationMap;
        }

        public OrganisationUserData Map(OrganisationUser source)
        {
            return new OrganisationUserData
            {
                Id = source.Id,
                UserId = source.UserId,
                OrganisationId = source.OrganisationId,
                OrganisationUserStatus =
                    (OrganisationUserStatus)
                        Enum.Parse(typeof(OrganisationUserStatus),
                            source.UserStatus.Value.ToString()),

                // Use existing mappers to map addresses and contact
                Organisation = source.Organisation != null
                    ? organisationMap.Map(source.Organisation)
                    : null
            };
        }
    }
}
