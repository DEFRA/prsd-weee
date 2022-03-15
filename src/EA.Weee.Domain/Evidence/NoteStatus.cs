namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class NoteStatus : Enumeration
    {
        public static readonly NoteStatus Draft = new NoteStatus(1, "Draft");
        public static readonly NoteStatus Submitted = new NoteStatus(2, "Submitted");

        protected NoteStatus()
        {
        }

        private NoteStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
