namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class Company
    {
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public ContactDetails RegisteredOffice { get; set; }

        public static Company Create(ISettings settings)
        {
            Company company = new Company();

            if (!settings.IgnoreStringLengthConditions)
            {
                company.CompanyName = RandomHelper.CreateRandomString(string.Empty, 1, 50); //255?
                company.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(8, 8);
            }
            else
            {
                company.CompanyName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                company.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
            }

            company.RegisteredOffice = ContactDetails.Create(settings, true);

            return company;
        }
    }
}
