namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using Weee.Security;
    using OrganisationStatus = Domain.Organisation.OrganisationStatus;

    public class GetManageUserDataAccess : IGetManageUserDataAccess
    {
        private readonly WeeeContext context;

        public GetManageUserDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<ManageUserData> GetCompetentAuthorityUser(Guid id)
        {
            var competentAuthorityUser = await
                (from u in context.Users
                 join cu in context.CompetentAuthorityUsers
                 on u.Id equals cu.UserId into caUsers
                 from caUser in caUsers
                 join ca in context.UKCompetentAuthorities on caUser.CompetentAuthorityId equals ca.Id
                 where caUser.Id == id
                 select new ManageUserData
                 {
                     UserId = u.Id,
                     Email = u.Email,
                     FirstName = u.FirstName,
                     LastName = u.Surname,
                     Id = caUser.Id,
                     OrganisationName = ca.Abbreviation,
                     UserStatus = (UserStatus)caUser.UserStatus.Value,
                     OrganisationId = caUser.CompetentAuthorityId,
                     Role = new Role
                     {
                         Name = caUser.Role.Name,
                         Description = caUser.Role.Description,
                     },
                     IsCompetentAuthorityUser = true
                 }).SingleOrDefaultAsync();

            return competentAuthorityUser;
        }

        public async Task<ManageUserData> GetOrganisationUser(Guid id)
        {
            var organisationsUser = await
                (from u in context.Users
                 join ou in context.OrganisationUsers on u.Id equals ou.UserId into idOrgUsers
                 from orgUser in idOrgUsers
                 join org in context.Organisations on orgUser.OrganisationId equals org.Id
                 where org.OrganisationStatus.Value == OrganisationStatus.Complete.Value && orgUser.Id == id
                 select new ManageUserData
                 {
                     UserId = u.Id,
                     Email = u.Email,
                     FirstName = u.FirstName,
                     LastName = u.Surname,
                     Id = orgUser.Id,
                     OrganisationName = org.Name ?? org.TradingName,
                     UserStatus = (UserStatus)orgUser.UserStatus.Value,
                     OrganisationId = orgUser.OrganisationId,
                     IsCompetentAuthorityUser = false
                 }).SingleOrDefaultAsync();

            return organisationsUser;
        }
    }
}
