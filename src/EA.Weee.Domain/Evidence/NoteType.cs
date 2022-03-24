namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class NoteType : Enumeration
    {
        public static readonly NoteType EvidenceNote = new NoteType(1, "Evidence");
        public static readonly NoteType TransferNote = new NoteType(2, "Transfer");

        protected NoteType()
        {
        }

        private NoteType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
