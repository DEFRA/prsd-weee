namespace EA.Weee.Core.Users
{
    using System;

    public class UserData
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}