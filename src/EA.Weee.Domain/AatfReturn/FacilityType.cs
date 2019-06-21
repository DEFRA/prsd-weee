namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using User;

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
