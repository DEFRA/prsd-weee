namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using Domain.Evidence;
    using Domain.Scheme;

    public class TransferNoteMapTransfer
    {
        public Scheme Scheme { get; private set; }

        public Note Note { get; private set; }

        public TransferNoteMapTransfer(Scheme scheme, Note note)
        {
            Condition.Requires(scheme).IsNotNull();
            Condition.Requires(note).IsNotNull();

            Scheme = scheme;
            Note = note;
        }
    }
}
