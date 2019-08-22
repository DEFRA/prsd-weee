﻿namespace EA.Weee.Email.EventHandlers
{
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using Domain.User;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrganisationUserRequestEventHandlerDataAccess : IOrganisationUserRequestEventHandlerDataAccess
    {
        private readonly WeeeContext context;

        public OrganisationUserRequestEventHandlerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<User> FetchUser(string userId)
        {
            return await context.Users
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();
        }

        public async Task<Organisation> FetchOrganisation(Guid orgId)
        {
            return await context.Organisations
                .Where(o => o.Id == orgId)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<OrganisationUser>> FetchActiveOrganisationUsers(Guid organisationId)
        {
            return await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == organisationId)
                .Where(ou => ou.UserStatus.Value == UserStatus.Active.Value)
                .Distinct()
                .OrderBy(u => u.User.Email)
                .ToListAsync();
        }

        public async Task<IEnumerable<UKCompetentAuthority>> FetchCompetentAuthorities()
        {
            return await context.UKCompetentAuthorities.ToListAsync();
        }
    }
}
