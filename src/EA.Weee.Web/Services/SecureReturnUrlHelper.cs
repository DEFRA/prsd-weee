namespace EA.Weee.Web.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class SecureReturnUrlHelper : ISecureReturnUrlHelper
    {
        private readonly ConfigurationService configurationService;

        public SecureReturnUrlHelper(ConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        public string GenerateSecureRandomString(int length = 32)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var token = Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            var hmac = ComputeHmac(token);
            var fullToken = $"{token}.{hmac}";

            return fullToken;
        }

        public bool ValidateSecureRandomString(string input)
        {
            if (string.IsNullOrEmpty(input) || !input.Contains('.'))
            {
                return false;
            }

            var parts = input.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            var token = parts[0];
            var providedHmac = parts[1];

            if (!token.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            {
                return false;
            }

            var computedHmac = ComputeHmac(token);
            return providedHmac.Equals(computedHmac);
        }

        private string ComputeHmac(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configurationService.CurrentConfiguration.GovUkPayTokenSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
            }
        }
    }
}