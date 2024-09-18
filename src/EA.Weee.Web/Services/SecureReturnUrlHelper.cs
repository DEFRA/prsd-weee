namespace EA.Weee.Web.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public class SecureReturnUrlHelper : ISecureReturnUrlHelper
    {
        public string GenerateSecureRandomString(int length = 32)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Substring(0, length);
            }
        }

        public bool ValidateSecureRandomString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return input.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
        }
    }
}