namespace EA.Weee.Requests.Registration
{
    using System;

    public class OrganisationRegistrationData
    {
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string TownOrCity { get; set; }
        public string CountyOrProvince { get; set; }
        public string Building { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public Guid CountryId { get; set; }
        public string EntityType { get; set; }
    }
}