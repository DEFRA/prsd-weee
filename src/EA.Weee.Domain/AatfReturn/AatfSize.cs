namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core.Domain;
    public class AatfSize : Enumeration
    {
        public static readonly AatfSize Small = new AatfSize(1, "Small");
        public static readonly AatfSize Large = new AatfSize(2, "Large");
        protected AatfSize()
        {
        }

        private AatfSize(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
