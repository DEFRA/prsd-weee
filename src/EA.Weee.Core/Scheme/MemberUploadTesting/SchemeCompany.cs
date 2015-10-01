namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class SchemeCompany
    {
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }

        public static SchemeCompany Create(ISettings settings)
        {
            SchemeCompany schemeCompany = new SchemeCompany();

            if (!settings.IgnoreStringLengthConditions)
            {
                schemeCompany.CompanyName = RandomHelper.CreateRandomString(string.Empty, 1, 255);
                schemeCompany.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(8, 8);
            }
            else
            {
                schemeCompany.CompanyName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                schemeCompany.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
            }

            return schemeCompany;
        }
    }
}
