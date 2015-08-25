namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class UserStatus : Enumeration
    {
        public static readonly UserStatus Pending = new UserStatus(1, "Pending");
        public static readonly UserStatus Active = new UserStatus(2, "Active");
        public static readonly UserStatus Rejected = new UserStatus(3, "Rejected");
        public static readonly UserStatus Inactive = new UserStatus(4, "Inactive");

        protected UserStatus()
        {
        }

        private UserStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}