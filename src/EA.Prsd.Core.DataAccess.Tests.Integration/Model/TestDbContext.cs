using System.Data.Entity;
using EA.Prsd.Core.DataAccess.Extensions;
using EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain;
using EA.Prsd.Core.Domain.Auditing;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() : base("name=Core.TestConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            var assembly = typeof(TestDbContext).Assembly;
            var coreAssembly = typeof(AuditorExtensions).Assembly;

            modelBuilder.Conventions.AddFromAssembly(assembly);
            modelBuilder.Configurations.AddFromAssembly(assembly);

            modelBuilder.Conventions.AddFromAssembly(coreAssembly);
            modelBuilder.Configurations.AddFromAssembly(coreAssembly);
        }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<SimpleEntity> SimpleEntities { get; set; }
        public virtual DbSet<EntityWithEnums> EnumEntities { get; set; }
        public virtual DbSet<EntityWithChildren> ParentEntities { get; set; }
        public virtual DbSet<EntityWithForeignId> ForeignIdEntities { get; set; }
    }
}
