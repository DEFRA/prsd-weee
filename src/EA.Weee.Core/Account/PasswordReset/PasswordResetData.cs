namespace EA.Weee.Core.Account.PasswordReset
{
    using System;

    public class PasswordResetData
    {
        public string NewPassword { get; private set; }

        public Guid UserId { get; private set; }

        public string Token { get; private set; }

        public PasswordResetData(Guid userId, string newPassword, string token)
        {
            UserId = userId;
            NewPassword = newPassword;
            Token = token;
        }
    }
}
