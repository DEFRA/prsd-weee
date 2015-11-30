namespace EA.Weee.Core.Search
{
    using System;

    public class OrganisationSearchResult : SearchResult
    {
        public Guid OrganisationId { get; set; }

        public string Name { get; set; }
    }
}
