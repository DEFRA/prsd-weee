namespace EA.Weee.RequestHandlers.Mappings
{
    using CuttingEdge.Conditions;
    using Domain.Evidence;

    public class TransferNoteMapTransfer
    {
        public Note Note { get; private set; }

        public TransferNoteMapTransfer(Note note)
        {
            Condition.Requires(note).IsNotNull();

            Note = note;
        }
    }
}
