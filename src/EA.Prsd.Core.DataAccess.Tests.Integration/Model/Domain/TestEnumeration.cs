namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    using EA.Prsd.Core.Domain;

    public class TestEnumeration : Enumeration
    {
        public static readonly TestEnumeration A = new TestEnumeration(1, "A");
        public static readonly TestEnumeration B = new TestEnumeration(2, "B");

        protected TestEnumeration()
        {
        }

        private TestEnumeration(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
