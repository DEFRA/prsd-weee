namespace EA.Weee.Sroc.Migration
{
    using System.Data.Entity;
    using System.Linq;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain;
    using Domain.Admin;
    using Domain.Charges;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using Domain.Security;
    using Domain.User;
    using Prsd.Core.DataAccess.Extensions;
    using Prsd.Core.Domain;
    using Prsd.Core.Domain.Auditing;

    public class WeeeMigrationContext : DbContext
    {
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<OrganisationUser> OrganisationUsers { get; set; }

        public virtual DbSet<UKCompetentAuthority> UKCompetentAuthorities { get; set; }

        public virtual DbSet<MemberUpload> MemberUploads { get; set; }

        public virtual DbSet<MemberUploadError> MemberUploadErrors { get; set; }

        public virtual DbSet<Scheme> Schemes { get; set; }

        /// <summary>
        /// Registered producers including items marked as removed.
        /// </summary>
        internal virtual DbSet<RegisteredProducer> AllRegisteredProducers { get; set; }

        /// <summary>
        /// Registered producers excluding records marked as removed.
        /// </summary>
        public IQueryable<RegisteredProducer> RegisteredProducers
        {
            get
            {
                return AllRegisteredProducers
                    .AsQueryable()
                    .Where(rp => !rp.Removed);
            }
        }

        /// <summary>
        /// Registered producers marked as removed.
        /// </summary>
        public virtual IQueryable<RegisteredProducer> RemovedRegisteredProducers
        {
            get
            {
                return AllRegisteredProducers
                    .AsQueryable()
                    .Where(rp => rp.Removed);
            }
        }

        /// <summary>
        /// Producer submissions including removed producers.
        /// </summary>
        public virtual DbSet<ProducerSubmission> AllProducerSubmissions { get; set; }

        /// <summary>
        /// Producer submissions excluding removed producers.
        /// </summary>
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

        public virtual DbSet<Role> Roles { get; set; }

        public virtual IStoredProcedures StoredProcedures { get; private set; }

        public WeeeMigrationContext()
            : base("name=Weee.DefaultConnection")
        {
            Database.SetInitializer<WeeeMigrationContext>(null);

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
    }
}
