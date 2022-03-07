namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Mappings
{
    using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;
    using System.Data.Entity.ModelConfiguration;

    internal class EntityWithForeignIdMapping : EntityTypeConfiguration<EntityWithForeignId>
    {
        public EntityWithForeignIdMapping()
        {
            ToTable("EntityWithForeignId", "Test");
        }
    }
}