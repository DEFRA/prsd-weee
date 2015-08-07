namespace EA.Weee.Domain.Organisation
{
    using Prsd.Core.Domain;

    public class SchemeStatus : Enumeration
    {
        public static readonly SchemeStatus Incomplete = new SchemeStatus(1, "Incomplete");
        public static readonly SchemeStatus Pending = new SchemeStatus(2, "Pending");
        public static readonly SchemeStatus Approved = new SchemeStatus(3, "Approved");
        public static readonly SchemeStatus Refused = new SchemeStatus(4, "Refused");
        public static readonly SchemeStatus Withdrawn = new SchemeStatus(5, "Withdrawn");

        protected SchemeStatus()
        {
        }

        private SchemeStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
