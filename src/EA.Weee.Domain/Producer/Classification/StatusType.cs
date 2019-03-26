namespace EA.Weee.Domain.Producer.Classification
{
    using Prsd.Core.Domain;

    public class StatusType : Enumeration
    {
        public static readonly StatusType Insert = new StatusType(0, "Insert");
        public static readonly StatusType Amendment = new StatusType(1, "Amendment");

        protected StatusType()
        {
        }

        private StatusType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}