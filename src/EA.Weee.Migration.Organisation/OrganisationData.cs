namespace EA.Weee.Migration.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrganisationData
    {
        public OrganisationData()
        {
        }

        public OrganisationData(int rowNumber, string name, string tradingName, OrganisationType? organisationType, string registrationNumber, AddressData addressData)
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

        public AddressData AddressData { get; set; }
    }
}
