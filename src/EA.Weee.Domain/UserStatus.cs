namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class UserStatus : Enumeration
    {
        public static readonly UserStatus Pending = new UserStatus(1, "Pending");
        public static readonly UserStatus Approved = new UserStatus(2, "Approved");
        public static readonly UserStatus Refused = new UserStatus(3, "Refused");
        public static readonly UserStatus Inactive = new UserStatus(4, "Inactive");

        protected UserStatus()
        {
        }

        private UserStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}