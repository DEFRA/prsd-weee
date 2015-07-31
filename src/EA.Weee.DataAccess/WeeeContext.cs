namespace EA.Weee.DataAccess
{
    using System.Data.Entity;
    using System.Linq;
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

            bool needToRefreshProducersIsCurrentForComplianceYear
                = NeedToRefreshProducersIsCurrentForComplianceYear();
            
            int result;
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    result = base.SaveChanges();

                    if (needToRefreshProducersIsCurrentForComplianceYear)
                    {
                        // Refresh the [IsCurrentForComplianceYear] column in [Producer].[Producer].
                        Database.ExecuteSqlCommand("EXEC [Producer].[sppRefreshProducerIsCurrent");
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.SetEntityId();
            //this.DeleteRemovedRelationships();
            this.AuditChanges(userContext.UserId);

            bool needToRefreshProducersIsCurrentForComplianceYear
                = NeedToRefreshProducersIsCurrentForComplianceYear();

            int result;
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    result = await base.SaveChangesAsync(cancellationToken);

                    if (needToRefreshProducersIsCurrentForComplianceYear)
                    {
                        // Refresh the [IsCurrentForComplianceYear] column in [Producer].[Producer].
                        await Database.ExecuteSqlCommandAsync(
                            "EXEC [Producer].[sppRefreshProducerIsCurrent]", cancellationToken);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the current set of changes being tracked by the context
        /// require that the [IsCurrentForComplianceYear] column needs to be refreshed
        /// for all producers.
        /// 
        /// This will be true if any rows are added, modified or deleted from the [Producer]
        /// table, or if any entry in the [MemberUpload] table has been updated.
        /// </summary>
        /// <returns></returns>
        private bool NeedToRefreshProducersIsCurrentForComplianceYear()
        {
            bool anyProducerUpdates = ChangeTracker.Entries()
                .Where(e => e.Entity is Producer)
                .Where(e => (e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted))
                .Any();

            bool anyMemberUploadUpdates = ChangeTracker.Entries()
                .Where(e => e.Entity is MemberUpload)
                .Where(e => e.State == EntityState.Modified)
                .Any();

            return anyProducerUpdates || anyMemberUploadUpdates;
        }

        public void DeleteOnCommit(Entity entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }
    }
}