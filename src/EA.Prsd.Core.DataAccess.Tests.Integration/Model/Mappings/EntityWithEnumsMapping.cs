using System.Data.Entity.ModelConfiguration;
using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Mappings
{
    internal class EntityWithEnumsMapping : EntityTypeConfiguration<EntityWithEnums>
    {
        public EntityWithEnumsMapping()
        {
            ToTable("EntityWithEnums", "Test");
        }
    }
}
