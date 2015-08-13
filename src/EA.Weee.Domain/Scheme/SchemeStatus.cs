namespace EA.Weee.Domain.Scheme
{
    using Prsd.Core.Domain;

    public class SchemeStatus : Enumeration
    {
        public static readonly SchemeStatus Pending = new SchemeStatus(1, "Pending");
        public static readonly SchemeStatus Approved = new SchemeStatus(2, "Approved");
        public static readonly SchemeStatus Rejected = new SchemeStatus(3, "Rejected");

        protected SchemeStatus()
        {
        }

        private SchemeStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
