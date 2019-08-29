using System.Data.Entity.ModelConfiguration;
using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Mappings
{
    internal class SimpleEntityMapping : EntityTypeConfiguration<SimpleEntity>
    {
        public SimpleEntityMapping()
        {
            ToTable("SimpleEntity", "Test");
        }
    }
}
