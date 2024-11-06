namespace EA.Weee.Web.Services
{
    using EA.Prsd.Core;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class PaymentReferenceGenerator : IPaymentReferenceGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly char[] CharArray = Chars.ToCharArray();

        public string GeneratePaymentReference(int length = 20)
        {
            if (length < 10 || length > 255)
            {
                throw new ArgumentException("Length must be between 10 and 255");
            }

            var builder = new StringBuilder("WEEE");
            builder.Append(SystemTime.UtcNow.Year);
            builder.Append((DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000).ToString("D6"));

            var remainingLength = length - builder.Length;

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                var randomNumber = new byte[1];
                for (var i = 0; i < remainingLength; i++)
                {
                    randomNumberGenerator.GetBytes(randomNumber);
                    builder.Append(CharArray[randomNumber[0] % (byte)CharArray.Length]);
                }
            }

            return builder.ToString();
        }

        public string GeneratePaymentReferenceWithSeparators(int length = 20)
        {
            if (length < 14 || length > 255)
            {
                throw new ArgumentException("Length must be between 14 and 255");
            }

            var baseReference = GeneratePaymentReference(length - 3);

            return $"{baseReference.Substring(0, 4)}-" +
                   $"{baseReference.Substring(4, 4)}-" +
                   $"{baseReference.Substring(8, 6)}-" +
                   $"{baseReference.Substring(14)}";
        }
    }
}