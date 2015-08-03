using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class Address
    {
        public string PrimaryName { get; set; }
        public string SecondaryName { get; set; }
        public string StreetName { get; set; }
        public string Town { get; set; }
        public string Locality { get; set; }
        public string AdministrativeArea { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }

        public Address()
        {
        }

        public static Address Create(IAddressSettings settings)
        {
            Address address = new Address();

            if (!settings.IgnoreStringLengthConditions)
            {
                address.PrimaryName = RandomHelper.CreateRandomString(string.Empty, 1, 100);
                address.SecondaryName = RandomHelper.CreateRandomString(string.Empty, 0, 100);
                address.StreetName = RandomHelper.CreateRandomString(string.Empty, 0, 50); //100?
                address.Town = RandomHelper.CreateRandomString(string.Empty, 0, 30);
                address.Locality = RandomHelper.CreateRandomString(string.Empty, 0, 35);
                address.AdministrativeArea = RandomHelper.CreateRandomString(string.Empty, 0, 30);
            }
            else
            {
                address.PrimaryName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                address.SecondaryName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                address.StreetName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                address.Town = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                address.Locality = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                address.AdministrativeArea = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
            }
                
            address.Country = "UK - ENGLAND";
            address.PostCode = "GU22 7UY";

            return address;
        }
    }
}
