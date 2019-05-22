namespace EA.Weee.Migration.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrganisationData
    {
        public int RowNumber { get; set; }

        public string Name { get; set; }

        public string TradingName { get; set; }

        public OrganisationType? OrganisationType { get; set; }

        public string RegistrationNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string TownOrCity { get; set; }

        public string CountyOrRegion { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }
    }
}
