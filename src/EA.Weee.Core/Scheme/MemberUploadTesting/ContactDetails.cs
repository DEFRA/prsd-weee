namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class ContactDetails
    {
        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string PhoneLandLine { get; set; }
        public string PhoneMobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Address Address { get; private set; }

        public ContactDetails()
        {
            Address = new Address();
        }

        public static ContactDetails Create(ISettings settings, bool isUkBased)
        {
            ContactDetails contactDetails = new ContactDetails();

            if (!settings.IgnoreStringLengthConditions)
            {
                contactDetails.Title = RandomHelper.CreateRandomString("Title ", 1, 35);
                contactDetails.Forename = RandomHelper.CreateRandomString("Forename ", 1, 35);
                contactDetails.Surname = RandomHelper.CreateRandomString("Surname ", 1, 35);
                contactDetails.PhoneLandLine = RandomHelper.CreateRandomStringOfNumbers(10, 35);
                contactDetails.PhoneMobile = RandomHelper.CreateRandomStringOfNumbers(10, 35);
                contactDetails.Fax = RandomHelper.CreateRandomStringOfNumbers(10, 35);
                contactDetails.Email = RandomHelper.CreateRandomString("WEEE_", 10, 15, false) + "@environment-agency.gov.uk";
            }
            else
            {
                contactDetails.Title = RandomHelper.CreateRandomString("Title ", 0, 1000);
                contactDetails.Forename = RandomHelper.CreateRandomString("Forename ", 0, 1000);
                contactDetails.Surname = RandomHelper.CreateRandomString("Surname ", 0, 1000);
                contactDetails.PhoneLandLine = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
                contactDetails.PhoneMobile = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
                contactDetails.Fax = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
                contactDetails.Email = RandomHelper.CreateRandomString("WEEE_", 0, 1000, false) + "@environment-agency.gov.uk";
            }

            contactDetails.Address = Address.Create(settings, isUkBased);

            return contactDetails;
        }
    }
}
