namespace EA.Weee.Domain.User
{
    using System;

    public class PasswordHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
    }
}
