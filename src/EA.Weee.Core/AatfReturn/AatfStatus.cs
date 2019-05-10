namespace EA.Weee.Core.AatfReturn
{
    using Prsd.Core.Domain;

    public class AatfStatus : Enumeration
    {
        public static readonly AatfStatus Approved = new AatfStatus(2, "Approved");
        protected AatfStatus()
        {
        }

        public AatfStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
