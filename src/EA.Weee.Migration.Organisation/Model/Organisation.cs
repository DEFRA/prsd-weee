namespace EA.Weee.Migration.Organisation.Model
{
    public class Organisation
    {
        public Organisation()
        {
        }

        public Organisation(int rowNumber, string name, string tradingName, OrganisationType? organisationType, string registrationNumber, Address addressData)
        {
            RowNumber = rowNumber;
            Name = name;
            TradingName = tradingName;
            OrganisationType = organisationType;
            RegistrationNumber = registrationNumber;
            AddressData = addressData;
        }

        public int RowNumber { get; set; }

        public string Name { get; set; }

        public string TradingName { get; set; }

        public OrganisationType? OrganisationType { get; set; }

        public string RegistrationNumber { get; set; }

        public Address AddressData { get; set; }
    }
}
