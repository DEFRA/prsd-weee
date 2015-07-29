namespace EA.Weee.Domain
{
    using Prsd.Core.Domain;

    public class ChargeBandType : Enumeration
    {
        public static readonly ChargeBandType A = new ChargeBandType(0, "A");
        public static readonly ChargeBandType B = new ChargeBandType(1, "B");
        public static readonly ChargeBandType C = new ChargeBandType(2, "C");
        public static readonly ChargeBandType D = new ChargeBandType(3, "D");
        public static readonly ChargeBandType E = new ChargeBandType(4, "E");

        protected ChargeBandType()
        {
        }

        private ChargeBandType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
