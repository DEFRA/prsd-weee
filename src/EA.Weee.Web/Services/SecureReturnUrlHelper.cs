namespace EA.Weee.Web.Services
{
    using CuttingEdge.Conditions;
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class SecureReturnUrlHelper : ISecureReturnUrlHelper
    {
        private readonly IAppConfiguration configuration;
        private readonly byte[] encryptionKey;
        private readonly byte[] encryptionIv;

        public SecureReturnUrlHelper(IAppConfiguration configuration)
        {
            Condition.Requires(configuration).IsNotNull();
            Condition.Requires(configuration.GovUkPayTokenSecret).IsNotNullOrWhiteSpace();
            Condition.Requires(configuration.GovUkPayTokenLifeTime).IsNotEqualTo(TimeSpan.Zero);

            this.configuration = configuration;

            using (var deriveBytes = new Rfc2898DeriveBytes(
                configuration.GovUkPayTokenSecret,
                Encoding.UTF8.GetBytes(configuration.GovUkPayTokenSalt),
                1000))
            {
                encryptionKey = deriveBytes.GetBytes(32);
                encryptionIv = deriveBytes.GetBytes(16);
            }
        }

        public string GenerateSecureRandomString(Guid guid, int length = 8)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var token = Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            var expiryTime = DateTime.UtcNow.Add(configuration.GovUkPayTokenLifeTime);

            var encryptedData = EncryptGuidAndTimestamp(guid, expiryTime);
            var combinedString = $"{token}.{encryptedData}";
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
            var encryptedData = parts[1];
            var providedHmac = parts[2];

            if (!token.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            {
                return (false, Guid.Empty);
            }

            var combinedString = $"{token}.{encryptedData}";
            var computedHmac = ComputeHmac(combinedString);

            if (!providedHmac.Equals(computedHmac))
            {
                return (false, Guid.Empty);
            }

            try
            {
                var (guid, expiryTime) = DecryptGuidAndTimestamp(encryptedData);

                if (DateTime.UtcNow > expiryTime)
                {
                    return (false, Guid.Empty);
                }

                return (true, guid);
            }
            catch
            {
                return (false, Guid.Empty);
            }
        }

        private string ComputeHmac(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(configuration.GovUkPayTokenSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
            }
        }

        private string EncryptGuidAndTimestamp(Guid guid, DateTime expiryTime)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = encryptionIv;

                using (var memoryStreamEncrypt = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor())
                    using (var cryptoStream = new CryptoStream(memoryStreamEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var writer = new BinaryWriter(cryptoStream))
                    {
                        writer.Write(guid.ToByteArray());
                        writer.Write(((DateTimeOffset)expiryTime).ToUnixTimeSeconds());
                    }

                    return Convert.ToBase64String(memoryStreamEncrypt.ToArray())
                        .Replace("+", "-").Replace("/", "_").TrimEnd('=');
                }
            }
        }

        private (Guid guid, DateTime expiryTime) DecryptGuidAndTimestamp(string encrypted)
        {
            var base64 = encrypted.Replace("-", "+").Replace("_", "/");
            var pad = 4 - (base64.Length % 4);
            if (pad != 4)
            {
                base64 += new string('=', pad);
            }

            using (var aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = encryptionIv;

                using (var memoryStreamDecrypt = new MemoryStream(Convert.FromBase64String(base64)))
                using (var decryptor = aes.CreateDecryptor())
                using (var cryptoStream = new CryptoStream(memoryStreamDecrypt, decryptor, CryptoStreamMode.Read))
                using (var reader = new BinaryReader(cryptoStream))
                {
                    var guidBytes = reader.ReadBytes(16);
                    var timestamp = reader.ReadInt64();

                    return (new Guid(guidBytes), DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime);
                }
            }
        }
    }
}