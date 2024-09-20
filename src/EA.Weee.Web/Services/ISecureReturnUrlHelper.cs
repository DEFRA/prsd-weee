namespace EA.Weee.Web.Services
{
    using System;

    public interface ISecureReturnUrlHelper
    {
        string GenerateSecureRandomString(Guid guid, int length = 16);

        (bool isValid, Guid guid) ValidateSecureRandomString(string input);
    }
}