namespace EA.Weee.RequestHandlers.Mappings
{
    using System;
    using Core.NewUser;
    using Core.Organisations;
    using Domain;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using OrganisationUserStatus = Core.Shared.UserStatus;

    public class OrganisationUserMap : IMap<OrganisationUser, OrganisationUserData>
    {
        private readonly IMap<Organisation, OrganisationData> organisationMap;

        private readonly IMap<User, UserData> userMap;

        public OrganisationUserMap(IMap<Organisation, OrganisationData> organisationMap, IMap<User, UserData> userMap)
        {
            this.organisationMap = organisationMap;
            this.userMap = userMap;
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
                            source.OrganisationUserStatus.Value.ToString()),

                // Use existing mappers to map addresses and contact
                Organisation = source.Organisation != null
                    ? organisationMap.Map(source.Organisation)
                    : null,

                User = source.User != null ? userMap.Map(source.User) : null
            };
        }
    }
}
