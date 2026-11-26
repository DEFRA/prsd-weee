namespace EA.Weee.Api.Client.Entities
{
    using System;

    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
