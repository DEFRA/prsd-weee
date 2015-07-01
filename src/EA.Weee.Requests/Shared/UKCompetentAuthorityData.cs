namespace EA.Weee.Requests.Shared
{
    using System;

    public class UKCompetentAuthorityData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public Guid CountryId { get; set; }
    }
}