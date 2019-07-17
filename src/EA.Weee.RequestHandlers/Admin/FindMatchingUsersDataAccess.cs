namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.User;
    using OrganisationStatus = Domain.Organisation.OrganisationStatus;

    public class FindMatchingUsersDataAccess : IFindMatchingUsersDataAccess
    {
        private readonly WeeeContext context;

        public FindMatchingUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<UserSearchData[]> GetCompetentAuthorityUsers(UserFilter filter)
        {
            var competentAuthorityUsers = await(
                    from u in context.Users
                    join cu in context.CompetentAuthorityUsers on u.Id equals cu.UserId into caUsers
                    from caUser in caUsers
                    join ca in context.UKCompetentAuthorities on caUser.CompetentAuthorityId equals ca.Id
                    where (filter.Status == null || filter.Status == (UserStatus)caUser.UserStatus.Value)
                    && (filter.OrganisationName == null || filter.OrganisationName.Trim() == string.Empty || ca.Abbreviation.ToLower().Contains(filter.OrganisationName.ToLower()))
                    && (filter.Name == null || filter.Name.Trim() == string.Empty || (u.FirstName + " " + u.Surname).ToLower().Contains(filter.Name.ToLower()))
                    select new UserSearchData
                    {
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Id = u.Id,
                        LastName = u.Surname,
                        OrganisationName = ca.Abbreviation,
                        Status = (UserStatus)caUser.UserStatus.Value,
                        OrganisationUserId = caUser.Id,
                        IsCompetentAuthorityUser = true,
                        Role = caUser.Role.Description
                    }).ToArrayAsync();

            return competentAuthorityUsers;
        }

        public async Task<UserSearchData[]> GetOrganisationUsers(UserFilter filter)
        {
            var result = await(
                    from u in context.Users
                    join ou in context.OrganisationUsers on u.Id equals ou.UserId into idOrgUsers
                    from orgUser in idOrgUsers
                    join org in context.Organisations on orgUser.OrganisationId equals org.Id
                    where org.OrganisationStatus.Value == OrganisationStatus.Complete.Value
                    && (filter.Status == null || filter.Status == (UserStatus)orgUser.UserStatus.Value)
                    && ((org.Name != null && (filter.OrganisationName == null || filter.OrganisationName.Trim() == string.Empty || org.Name.ToLower().Contains(filter.OrganisationName.ToLower())))
                        || (org.Name == null && org.TradingName != null && (filter.OrganisationName == null || filter.OrganisationName.Trim() == string.Empty || org.TradingName.ToLower().Contains(filter.OrganisationName.ToLower()))))
                    && (filter.Name == null || filter.Name.Trim() == string.Empty || (u.FirstName + " " + u.Surname).ToLower().Contains(filter.Name.ToLower()))
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
                        OrganisationId = org.Id,
                        Role = "N/A"
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
