namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using EA.Weee.Domain.Evidence;

    public class EvidenceNoteMappingTransfer
    {
        public Note Note { get; set; }

        public SchemeData SchemeData { get; set; }
    }
}
