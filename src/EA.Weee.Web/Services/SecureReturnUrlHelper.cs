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

        public string GenerateSecureRandomString(Guid guid, int length = 16)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var token = Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            var guidString = guid.ToString("N");
            var combinedString = $"{token}.{guidString}";
            var hmac = ComputeHmac(combinedString);

            var fullToken = $"{combinedString}.{hmac}";
            return fullToken;
        }

        public (bool isValid, Guid guid) ValidateSecureRandomString(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Count(c => c == '.') != 2)
            {
                return (false, Guid.Empty);
            }

            var parts = input.Split('.');
            if (parts.Length != 3)
            {
                return (false, Guid.Empty);
            }

            var token = parts[0];
            var guidString = parts[1];
            var providedHmac = parts[2];

            if (!token.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            {
                return (false, Guid.Empty);
            }

            if (!Guid.TryParseExact(guidString, "N", out var guid))
            {
                return (false, Guid.Empty);
            }

            var combinedString = $"{token}.{guidString}";
            var computedHmac = ComputeHmac(combinedString);

            return (providedHmac.Equals(computedHmac), guid);
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