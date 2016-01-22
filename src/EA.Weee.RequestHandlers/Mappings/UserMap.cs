namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Users;
    using Domain.User;
    using Prsd.Core.Mapper;

    public class UserMap : IMap<User, UserData>
    {
        public UserData Map(User source)
        {
            return new UserData
            {
                Id = source.Id,
                Email = source.Email,
                FirstName = source.FirstName,
                Surname = source.Surname
            };
        }
    }
}
