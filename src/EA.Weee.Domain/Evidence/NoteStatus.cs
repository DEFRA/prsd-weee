namespace EA.Weee.Domain.Evidence
{
    using Prsd.Core.Domain;

    public class NoteStatus : Enumeration
    {
        public static readonly NoteStatus Draft = new NoteStatus(1, "Draft");
        public static readonly NoteStatus Submitted = new NoteStatus(2, "Submitted");
        public static readonly NoteStatus Approved = new NoteStatus(3, "Approved");
        public static readonly NoteStatus Rejected = new NoteStatus(4, "Rejected");
        public static readonly NoteStatus Void = new NoteStatus(5, "Void");
        public static readonly NoteStatus Returned = new NoteStatus(6, "Returned");
        public static readonly NoteStatus Cancelled = new NoteStatus(7, "Cancelled");

        protected NoteStatus()
        {
        }

        private NoteStatus(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
