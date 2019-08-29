using EA.Prsd.Core.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    public class EntityWithEnums : Entity
    {
        public TestEnumeration TestEnumerationValue { get; set; }

        public SimpleEnum SimpleEnumValue { get; set; }

        public EntityWithEnums()
        {
            TestEnumerationValue = TestEnumeration.A;
            SimpleEnumValue = SimpleEnum.Value0;
        }

        public EntityWithEnums(TestEnumeration testEnumeration, SimpleEnum simpleEnumValue)
        {
            TestEnumerationValue = testEnumeration;
            SimpleEnumValue = simpleEnumValue;
        }
    }
}
