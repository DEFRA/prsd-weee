namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class NoteType : Enumeration
    {
        public static readonly NoteType EvidenceNote = new NoteType(1, "E");
        public static readonly NoteType TransferNote = new NoteType(2, "T");

        protected NoteType()
        {
        }

        private NoteType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
