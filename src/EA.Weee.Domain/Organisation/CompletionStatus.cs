namespace EA.Weee.Domain.Organisation
{
    using EA.Prsd.Core.Domain;

    public class CompletionStatus : Enumeration
    {
        public static readonly CompletionStatus Incomplete = new CompletionStatus(1, "Incomplete");
        public static readonly CompletionStatus Complete = new CompletionStatus(2, "Complete");

        protected CompletionStatus()
        {
        }

        private CompletionStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
