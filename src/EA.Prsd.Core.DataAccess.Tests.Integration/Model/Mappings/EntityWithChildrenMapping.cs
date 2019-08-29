using System.Data.Entity.ModelConfiguration;
using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Mappings
{
    internal class EntityWithChildrenMapping : EntityTypeConfiguration<EntityWithChildren>
    {
        public EntityWithChildrenMapping()
        {
            ToTable("EntityWithChildren", "Test");
        }
    }
}
