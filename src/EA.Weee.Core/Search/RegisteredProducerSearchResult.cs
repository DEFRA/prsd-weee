namespace EA.Weee.Core.Search
{
    using System;

    public abstract class RegisteredProducerSearchResult : SearchResult
    {
        public string RegistrationNumber { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
