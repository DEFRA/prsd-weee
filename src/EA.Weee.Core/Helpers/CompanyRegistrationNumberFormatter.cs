namespace EA.Weee.Core.Helpers
{
    public static class CompanyRegistrationNumberFormatter
    {
        public static string FormatCompanyRegistrationNumber(string companyRegistrationNumber)
        {
            string result = null;

            if (companyRegistrationNumber != null)
            {
                result = companyRegistrationNumber
                    .Replace(" ", string.Empty)
                    .TrimStart('0');
            }

            return result;
        }
    }
}
