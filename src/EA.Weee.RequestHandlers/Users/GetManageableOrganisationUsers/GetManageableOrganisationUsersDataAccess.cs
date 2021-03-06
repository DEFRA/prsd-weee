﻿namespace EA.Weee.RequestHandlers.Users.GetManageableOrganisationUsers
{
    using DataAccess;
    using Domain.Organisation;
    using Domain.User;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetManageableOrganisationUsersDataAccess : IGetManageableOrganisationUsersDataAccess
    {
        private readonly WeeeContext context;

        public GetManageableOrganisationUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<OrganisationUser>> GetManageableUsers(Guid organisationId)
        {
            var organisationUsers = await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == organisationId)
                .ToArrayAsync();

            var filteredOrganisationUsers = new List<OrganisationUser>();
            foreach (var organisationUser in organisationUsers)
            {
                if (!filteredOrganisationUsers
                    .Select(ou => ou.UserId)
                    .Contains(organisationUser.UserId))
                {
                    if (organisationUser.UserStatus != UserStatus.Rejected
                        || organisationUsers
                            .Where(ou => ou.UserId == organisationUser.UserId)
                            .All(ou => ou.UserStatus == UserStatus.Rejected))
                    {
                        filteredOrganisationUsers.Add(organisationUser);
                    }
                }
            }

            return filteredOrganisationUsers;
        }
    }
}
