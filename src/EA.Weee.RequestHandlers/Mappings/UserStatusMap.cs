namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.User;
    using Prsd.Core.Mapper;

    public class UserStatusMap : IMap<UserStatus, Core.Shared.UserStatus>
    {
        public Core.Shared.UserStatus Map(UserStatus source)
        {
            if (source == UserStatus.Active)
            {
                return Core.Shared.UserStatus.Active;
            }

            if (source == UserStatus.Rejected)
            {
                return Core.Shared.UserStatus.Rejected;
            }
            if (source == UserStatus.Pending)
            {
                return Core.Shared.UserStatus.Pending;
            }

            return Core.Shared.UserStatus.Inactive;
        }
    }
}
