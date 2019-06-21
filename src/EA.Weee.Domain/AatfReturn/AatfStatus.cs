namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;

    public class AatfStatus : Enumeration
    {
        public static readonly AatfStatus Approved = new AatfStatus(1, "Approved");
        public static readonly AatfStatus Suspended = new AatfStatus(2, "Suspended");
        public static readonly AatfStatus Cancelled = new AatfStatus(3, "Cancelled");
        protected AatfStatus()
        {
        }

        private AatfStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
