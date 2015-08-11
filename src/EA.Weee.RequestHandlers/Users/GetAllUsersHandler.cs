namespace EA.Weee.RequestHandlers.Users
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using OrganisationUserStatus = Core.Shared.UserStatus;

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
                            OrganisationName = ou.Organisation.TradingName,
                            Status = (OrganisationUserStatus)ou.OrganisationUserStatus.Value
                        }).ToListAsync();
            
            users.AddRange(organisationsUsers.Select(user => user).ToList());

            //TODO internal admin users
          
            return users.OrderBy(u => u.FullName).ToList();
        }
    }
}
