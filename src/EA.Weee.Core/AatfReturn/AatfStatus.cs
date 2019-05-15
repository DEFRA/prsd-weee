namespace EA.Weee.Core.AatfReturn
{
    using Prsd.Core.Domain;

    public class AatfStatus : Enumeration
    {
        public static readonly AatfStatus Approved = new AatfStatus(2, "Approved");
        public static readonly AatfStatus Suspended = new AatfStatus(3, "Suspended");
        public static readonly AatfStatus Cancelled = new AatfStatus(4, "Cancelled");
        protected AatfStatus()
        {
        }

        public AatfStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
