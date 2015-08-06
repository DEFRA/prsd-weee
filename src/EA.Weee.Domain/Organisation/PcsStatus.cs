namespace EA.Weee.Domain.Organisation
{
    using Prsd.Core.Domain;

    public class PcsStatus : Enumeration
    {
        public static readonly PcsStatus Incomplete = new PcsStatus(1, "Incomplete");
        public static readonly PcsStatus Pending = new PcsStatus(2, "Pending");
        public static readonly PcsStatus Approved = new PcsStatus(3, "Approved");
        public static readonly PcsStatus Refused = new PcsStatus(4, "Refused");
        public static readonly PcsStatus Withdrawn = new PcsStatus(5, "Withdrawn");

        protected PcsStatus()
        {
        }

        private PcsStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
