namespace EA.Weee.Core.Search
{
    public abstract class RegisteredProducerSearchResult : SearchResult
    {
        public string RegistrationNumber { get; set; }

        public string Name { get; set; }
    }
}
