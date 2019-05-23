using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Migration.Organisation
{
    public class AddressData
    {
        public AddressData()
        {
        }

        public AddressData(
            string address1,
            string address2,
            string townOrCity,
            string countyOrRegion,
            string postcode,
            string countryName,
            string telephone,
            string email)
        {
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            CountryName = countryName;
            Telephone = telephone;
            Email = email;
        }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string TownOrCity { get; set; }

        public string CountyOrRegion { get; set; }

        public string Postcode { get; set; }

        public Guid CountryId { get; set; }

        public string CountryName { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }
    }
}
