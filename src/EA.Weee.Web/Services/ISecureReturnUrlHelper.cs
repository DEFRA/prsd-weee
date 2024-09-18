namespace EA.Weee.Web.Services
{
    public interface ISecureReturnUrlHelper
    {
        string GenerateSecureRandomString(int length = 32);

        bool ValidateSecureRandomString(string input);
    }
}