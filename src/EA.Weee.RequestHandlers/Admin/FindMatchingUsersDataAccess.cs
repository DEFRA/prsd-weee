namespace EA.Weee.RequestHandlers.Admin
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationStatus = Domain.Organisation.OrganisationStatus;

    public class FindMatchingUsersDataAccess : IFindMatchingUsersDataAccess
    {
        private readonly WeeeContext context;

        public FindMatchingUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<UserSearchData[]> GetCompetentAuthorityUsers()
        {
            var competentAuthorityUsers = await(
                    from u in context.Users
                    join cu in context.CompetentAuthorityUsers on u.Id equals cu.UserId into caUsers
                    from caUser in caUsers
                    join ca in context.UKCompetentAuthorities on caUser.CompetentAuthorityId equals ca.Id
                    select new UserSearchData
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Id = u.Id,
                        LastName = u.Surname,
                        OrganisationName = ca.Abbreviation,
                        Status = (UserStatus)caUser.UserStatus.Value,
                        OrganisationUserId = caUser.Id,
                        IsCompetentAuthorityUser = true
                    }).ToArrayAsync();

            return competentAuthorityUsers;
        }

        public async Task<UserSearchData[]> GetOrganisationUsers()
        {
            var result = await(
                    from u in context.Users
                    join ou in context.OrganisationUsers on u.Id equals ou.UserId into idOrgUsers
                    from orgUser in idOrgUsers
                    join org in context.Organisations on orgUser.OrganisationId equals org.Id
                    where org.OrganisationStatus.Value == OrganisationStatus.Complete.Value
                    select new UserSearchData
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Id = u.Id,
                        LastName = u.Surname,
                        OrganisationName = org.Name ?? org.TradingName,
                        Status = (UserStatus)orgUser.UserStatus.Value,
                        OrganisationUserId = orgUser.Id,
                        IsCompetentAuthorityUser = false,
                        OrganisationId = org.Id
                    }).ToListAsync();

            // If a user has been rejected previously, there can be multiple organisation user records for the same user and organisation
            // so we need to only include the relevent (i.e. the current status for this user and this organisation) organisation user
            var organisationUsers = new List<UserSearchData>();
            foreach (var organisationUser in result)
            {
                var potentialMultipleJoinRequests = result
                    .Where(ou => ou.Id == organisationUser.Id && ou.OrganisationId == organisationUser.OrganisationId)
                    .ToList();

                if (potentialMultipleJoinRequests.Count > 1)
                {
                    var nonRejectedJoinRequest = potentialMultipleJoinRequests
                        .SingleOrDefault(ou => ou.Status != UserStatus.Rejected);

                    if (nonRejectedJoinRequest != null && !organisationUsers.Contains(nonRejectedJoinRequest))
                    {
                        organisationUsers.Add(nonRejectedJoinRequest);
                    }
                    else if (!organisationUsers.Any(ou => ou.OrganisationId == organisationUser.OrganisationId && ou.Id == organisationUser.Id))
                    {
                        organisationUsers.Add(organisationUser);
                    }
                }
                else
                {
                    organisationUsers.Add(organisationUser);
                }
            }

            return organisationUsers.ToArray();
        }
    }
}
