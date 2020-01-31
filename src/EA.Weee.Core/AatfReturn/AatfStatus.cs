namespace EA.Weee.Core.AatfReturn
{
    using System;
    using Prsd.Core.Domain;

    [Serializable]
    public class AatfStatus : Enumeration
    {
        public static readonly AatfStatus Approved = new AatfStatus(1, "Approved");
        public static readonly AatfStatus Suspended = new AatfStatus(2, "Suspended");
        public static readonly AatfStatus Cancelled = new AatfStatus(3, "Cancelled");
        protected AatfStatus()
        {
        }

        public AatfStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
