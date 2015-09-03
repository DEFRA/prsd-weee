namespace EA.Weee.Api.Client.Actions
{
    using Entities;

    interface IExistingUser
    {
        bool ResetPassword(PasswordResetData data);
    }
}
