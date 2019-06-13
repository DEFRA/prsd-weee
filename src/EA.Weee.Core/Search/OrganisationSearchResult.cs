namespace EA.Weee.Core.Search
{
    using EA.Weee.Core.Shared;
    using System;

    public class OrganisationSearchResult : SearchResult
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public AddressData Address { get; set; }
    }
}
