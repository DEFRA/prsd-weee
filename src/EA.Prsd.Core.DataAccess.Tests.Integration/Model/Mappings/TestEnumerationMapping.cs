namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Prsd.Core.DataAccess.Tests.Integration.Model.Domain;

    internal class TestEnumerationMapping : ComplexTypeConfiguration<TestEnumeration>
    {
        public TestEnumerationMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("TestEnumerationValue");
        }
    }
}
