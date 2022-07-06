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

        public bool IsBalancingScheme { get; set; }

        public string AddressString
        {
            get
            {
                string addressString = this.Address.Address1;

                if (this.Address.Address2 != null)
                {
                    addressString += ",<br/>" + this.Address.Address2;
                }

                addressString += ",<br/>" + this.Address.TownOrCity;

                if (this.Address.CountyOrRegion != null)
                {
                    addressString += ",<br/>" + this.Address.CountyOrRegion;
                }

                if (this.Address.Postcode != null)
                {
                    addressString += ",<br/>" + this.Address.Postcode;
                }

                addressString += ",<br/>" + this.Address.CountryName;

                return addressString;
            }
        }

        public string NameWithAddress
        {
            get
            {
                return string.Format("{0} ({1})", this.Name, this.AddressString.Replace("<br/>", " "));
            }
        }
    }
}
