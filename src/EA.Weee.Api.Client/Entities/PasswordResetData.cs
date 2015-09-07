namespace EA.Weee.Api.Client.Entities
{
    using System;

    public class PasswordResetData
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }
    }
}
