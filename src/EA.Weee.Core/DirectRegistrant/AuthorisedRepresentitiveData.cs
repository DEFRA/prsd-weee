namespace EA.Weee.Core.DirectRegistrant
{
    using System;

    public class AuthorisedRepresentitiveData
    {
        public string CompanyName {get; set; }

        public string BusinessTradingName {get; set; }

        public string Address1 {get; set; }

        public string Address2 { get; set; }

        public string TownOrCity { get; set; }

        public string CountyOrRegion { get; set; }

        public string Postcode { get; set; }

        public Guid CountryId { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }

        public string CountryName { get; set; }
    }
}
