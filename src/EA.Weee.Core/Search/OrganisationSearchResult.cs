namespace EA.Weee.Core.Search
{
    using EA.Weee.Core.Shared;
    using System;

    public class OrganisationSearchResult : SearchResult
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public AddressData Address { get; set; }

        public int AatfCount { get; set; }

        public int AeCount { get; set; }

        public int PcsCount { get; set; }

        public string NameWithAddress
        {
            get
            {
                string addressString = this.Address.Address1;

                if (this.Address.Address2 != null)
                {
                    addressString += ", " + this.Address.Address2;
                }

                addressString += ", " + this.Address.TownOrCity;

                if (this.Address.CountyOrRegion != null)
                {
                    addressString += ", " + this.Address.CountyOrRegion;
                }

                if (this.Address.Postcode != null)
                {
                    addressString += ", " + this.Address.Postcode;
                }

                addressString += ", " + this.Address.CountryName;

                return string.Format("{0} ({1})", this.Name, addressString);
            }
        }
    }
}
