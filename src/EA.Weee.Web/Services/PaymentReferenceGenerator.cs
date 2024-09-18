namespace EA.Weee.Web.Services
{
    using System;
    using System.Text;

    public class PaymentReferenceGenerator : IPaymentReferenceGenerator
    {
        private static readonly Random Random = new Random();

        public string GeneratePaymentReference(int length = 20)
        {
            if (length < 10 || length > 255)
            {
                throw new ArgumentException("Length must be between 10 and 255");
            }

            var builder = new StringBuilder("WEEE");

            builder.Append(DateTime.UtcNow.Year);

            builder.Append(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000);

            var remainingLength = length - builder.Length;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            for (var i = 0; i < remainingLength; i++)
            {
                builder.Append(chars[Random.Next(chars.Length)]);
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

            // Insert separators
            return $"{baseReference.Substring(0, 4)}-" +
                   $"{baseReference.Substring(4, 4)}-" +
                   $"{baseReference.Substring(8, 6)}-" +
                   $"{baseReference.Substring(14)}";
        }
    }
}