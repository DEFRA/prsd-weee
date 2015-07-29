namespace EA.Weee.DataAccess
{
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using Domain.PCS;
    using Domain.Producer;
    using Prsd.Core.DataAccess.Extensions;
    using Prsd.Core.Domain;
    using Prsd.Core.Domain.Auditing;

    public class WeeeContext : DbContext
    {
        private readonly IUserContext userContext;

        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Country> Countries { get; set; }
        
        public virtual DbSet<OrganisationUser> OrganisationUsers { get; set; }

        public virtual DbSet<UKCompetentAuthority> UKCompetentAuthorities { get; set; }

        public virtual DbSet<MemberUpload> MemberUploads { get; set; }

        public virtual DbSet<Scheme> Schemes { get; set; }

        public virtual DbSet<Producer> Producers { get; set; }

        public virtual DbSet<MigratedProducer> MigratedProducers { get; set; }

        public virtual DbSet<ProducerChargeBand> ProducerChargeBands { get; set; }

        public WeeeContext(IUserContext userContext)
            : base("name=Weee.DefaultConnection")
        {
            this.userContext = userContext;
            Database.SetInitializer<WeeeContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assembly = typeof(WeeeContext).Assembly;
            var coreAssembly = typeof(AuditorExtensions).Assembly;

            modelBuilder.Conventions.AddFromAssembly(assembly);
            modelBuilder.Configurations.AddFromAssembly(assembly);

            modelBuilder.Conventions.AddFromAssembly(coreAssembly);
            modelBuilder.Configurations.AddFromAssembly(coreAssembly);
        }

        public override int SaveChanges()
        {
            this.SetEntityId();
            //this.DeleteRemovedRelationships();
            this.AuditChanges(userContext.UserId);

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.SetEntityId();
            //this.DeleteRemovedRelationships();
            this.AuditChanges(userContext.UserId);

            return base.SaveChangesAsync(cancellationToken);
        }

        public void DeleteOnCommit(Entity entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }
    }
}