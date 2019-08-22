namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;

    public class FacilityType : Enumeration
    {
        public static readonly FacilityType Aatf = new FacilityType(1, "AATF");
        public static readonly FacilityType Ae = new FacilityType(2, "AE");

        protected FacilityType()
        {
        }

        private FacilityType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
