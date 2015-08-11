namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using userStatus = Core.Shared.UserStatus;

    public class GetAllUsersHandler : IRequestHandler<GetAllUsers, List<UserSearchData>>
    {
          private readonly WeeeContext context;
   
          public GetAllUsersHandler(WeeeContext context)
            {
                this.context = context;
            }

        public async Task<List<UserSearchData>> HandleAsync(GetAllUsers message)
        {
            List<UserSearchData> users = new List<UserSearchData>();
            var organisationsUsers = await(from u in context.Users
                        join ou in context.OrganisationUsers on u.Id equals ou.UserId
                        select new UserSearchData
                        {
                            Email = u.Email,
                            FirstName = u.FirstName,
                            Id = u.Id,
                            LastName = u.Surname,
                            OrganisationName = ou.Organisation.Name ?? ou.Organisation.TradingName,
                            Status = (userStatus)ou.UserStatus.Value
                        }).ToListAsync();
            
            users.AddRange(organisationsUsers.Select(user => user).ToList());

            //internal admin users
            var competentAuthorityUsers = await(from u in context.Users
                                                 join cu in context.CompetentAuthorityUsers on u.Id equals cu.UserId
                                                 select new UserSearchData
                                                 {
                                                     Email = u.Email,
                                                     FirstName = u.FirstName,
                                                     Id = u.Id,
                                                     LastName = u.Surname,
                                                     OrganisationName = cu.CompetentAuthority.Abbreviation,
                                                     Status = (userStatus)cu.UserStatus.Value
                                                 }).ToListAsync();

            users.AddRange(competentAuthorityUsers.Select(interaluser => interaluser).ToList());
            return users.OrderBy(u => u.FullName).ToList();
        }
    }
}
