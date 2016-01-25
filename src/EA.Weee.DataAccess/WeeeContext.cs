namespace EA.Weee.DataAccess
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Admin;
    using Domain.Audit;
    using Domain.Charges;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using Domain.User;
    using Prsd.Core;
    using Prsd.Core.DataAccess.Extensions;
    using Prsd.Core.Domain;
    using Prsd.Core.Domain.Auditing;
    using StoredProcedure;

    public class WeeeContext : DbContext
    {
        private readonly IUserContext userContext;
        private readonly IEventDispatcher dispatcher;

        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<OrganisationUser> OrganisationUsers { get; set; }

        public virtual DbSet<UKCompetentAuthority> UKCompetentAuthorities { get; set; }

        public virtual DbSet<MemberUpload> MemberUploads { get; set; }

        public virtual DbSet<MemberUploadError> MemberUploadErrors { get; set; }

        public virtual DbSet<Scheme> Schemes { get; set; }

        internal virtual DbSet<RegisteredProducer> AllRegisteredProducers { get; set; }

        public IQueryable<RegisteredProducer> RegisteredProducers
        {
            get
            {
                return AllRegisteredProducers
                    .AsQueryable()
                    .Where(rp => !rp.Removed);
            }
        }

        public virtual IQueryable<RegisteredProducer> RemovedRegisteredProducers
        {
            get
            {
                return AllRegisteredProducers
                    .AsQueryable()
                    .Where(rp => rp.Removed);
            }
        }

        internal virtual DbSet<ProducerSubmission> AllProducerSubmissions { get; set; }

        public virtual IQueryable<ProducerSubmission> ProducerSubmissions
        {
            get
            {
                return AllProducerSubmissions
                    .AsQueryable()
                    .Where(ps => !ps.RegisteredProducer.Removed);
            }
        }

        public virtual IQueryable<ProducerSubmission> RemovedProducerSubmissions
        {
            get
            {
                return AllProducerSubmissions
                    .AsQueryable()
                    .Where(ps => ps.RegisteredProducer.Removed);
            }
        }

        public virtual DbSet<SystemData> SystemData { get; set; }

        public virtual DbSet<MigratedProducer> MigratedProducers { get; set; }

        public virtual DbSet<ChargeBandAmount> ChargeBandAmounts { get; set; }

        public virtual DbSet<CompetentAuthorityUser> CompetentAuthorityUsers { get; set; }

        public virtual DbSet<DataReturnUpload> DataReturnsUploads { get; set; }

        public virtual DbSet<DataReturnUploadError> DataReturnsUploadErrors { get; set; }

        public virtual DbSet<DataReturnVersion> DataReturnVersions { get; set; }

        public virtual DbSet<DataReturn> DataReturns { get; set; }

        public virtual DbSet<WeeeDeliveredAmount> WeeeDeliveredAmounts { get; set; }

        public virtual DbSet<AatfDeliveryLocation> AatfDeliveryLocations { get; set; }

        public virtual DbSet<AeDeliveryLocation> AeDeliveryLocations { get; set; }

        public virtual DbSet<WeeeCollectedReturnVersion> WeeeCollectedReturnVersions { get; set; }

        public virtual DbSet<WeeeCollectedAmount> WeeeCollectedAmounts { get; set; }

        public virtual DbSet<EeeOutputAmount> EeeOutputAmounts { get; set; }

        public virtual DbSet<InvoiceRun> InvoiceRuns { get; set; }

        public virtual DbSet<QuarterWindowTemplate> QuarterWindowTemplates { get; set; }

        public virtual IStoredProcedures StoredProcedures { get; private set; }

        public WeeeContext(IUserContext userContext, IEventDispatcher dispatcher)
            : base("name=Weee.DefaultConnection")
        {
            this.userContext = userContext;
            this.dispatcher = dispatcher;

            Database.SetInitializer<WeeeContext>(null);

            StoredProcedures = new StoredProcedures(this);

            // Set internal db sets
            AllRegisteredProducers = Set<RegisteredProducer>();
            AllProducerSubmissions = Set<ProducerSubmission>();
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
            bool alreadyHasTransaction = (this.Database.CurrentTransaction != null);

            this.SetEntityId();
            this.AuditChanges(userContext.UserId);
            AuditEntities();

            int result;
            if (alreadyHasTransaction)
            {
                result = base.SaveChanges();

                Task.Run(() => this.DispatchEvents(dispatcher)).Wait();
            }
            else
            {
                using (var transaction = Database.BeginTransaction())
                {
                    result = base.SaveChanges();

                    Task.Run(() => this.DispatchEvents(dispatcher)).Wait();

                    transaction.Commit();
                }
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            bool alreadyHasTransaction = (this.Database.CurrentTransaction != null);

            this.SetEntityId();
            this.AuditChanges(userContext.UserId);
            AuditEntities();

            int result;
            if (alreadyHasTransaction)
            {
                result = await base.SaveChangesAsync(cancellationToken);

                await this.DispatchEvents(dispatcher);
            }
            else
            {
                using (var transaction = Database.BeginTransaction())
                {
                    result = await base.SaveChangesAsync(cancellationToken);

                    await this.DispatchEvents(dispatcher);

                    transaction.Commit();
                }
            }

            return result;
        }

        public void DeleteOnCommit(Entity entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public string GetCurrentUser()
        {
            if (userContext != null)
            {
                return userContext.UserId.ToString();
            }
            return null;
        }

        private void AuditEntities()
        {
            foreach (var auditableEntity in ChangeTracker.Entries<AuditableEntity>())
            {
                if (auditableEntity.State == EntityState.Added)
                {
                    auditableEntity.Entity.CreatedById = userContext.UserId.ToString();
                    auditableEntity.Entity.CreatedDate = SystemTime.UtcNow;
                }
                else if (auditableEntity.State == EntityState.Modified)
                {
                    auditableEntity.Entity.UpdatedById = userContext.UserId.ToString();
                    auditableEntity.Entity.UpdatedDate = SystemTime.UtcNow;
                }
            }
        }
    }
}