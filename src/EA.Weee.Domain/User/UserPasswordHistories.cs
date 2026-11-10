namespace EA.Weee.Domain.User
{
    using EA.Prsd.Core;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserPasswordHistories
    {
        public UserPasswordHistories()
        {
        }

        public UserPasswordHistories(string userId, string passwordHash)
        {
            Guard.ArgumentNotNullOrEmpty(() => userId, userId);
            Guard.ArgumentNotNullOrEmpty(() => passwordHash, passwordHash);

            UserId = userId.Trim();
            PasswordHash = passwordHash.Trim();
        }

        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
