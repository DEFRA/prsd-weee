namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System.Data.Entity;
    using System.Linq;
    using DataAccess;
    using Domain.Charges;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Scheme;
    using Domain.User;
    using Prsd.Core.DataAccess.Extensions;

    public class WeeeMigrationContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

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
            get { return AllProducerSubmissions.AsQueryable(); }
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

        public virtual DbSet<ChargeBandAmount> ChargeBandAmounts { get; set; }

        public virtual DbSet<InvoiceRun> InvoiceRuns { get; set; }

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
