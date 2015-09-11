namespace EA.Weee.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Admin;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core.DataAccess.Extensions;
    using Prsd.Core.Domain;
    using Prsd.Core.Domain.Auditing;

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

        public virtual DbSet<Scheme> Schemes { get; set; }

        public virtual DbSet<Producer> Producers { get; set; }

        public virtual DbSet<SystemData> SystemData { get; set; }

        public virtual DbSet<MigratedProducer> MigratedProducers { get; set; }

        public virtual DbSet<ProducerChargeBand> ProducerChargeBands { get; set; }

        public virtual DbSet<CompetentAuthorityUser> CompetentAuthorityUsers { get; set; }

        public virtual IStoredProcedures StoredProcedures { get; private set; }

        public WeeeContext(IUserContext userContext, IEventDispatcher dispatcher)
            : base("name=Weee.DefaultConnection")
        {
            this.userContext = userContext;
            this.dispatcher = dispatcher;

            Database.SetInitializer<WeeeContext>(null);

            StoredProcedures = new StoredProcedures(this);
        }

        /// <summary>
        /// This constructor should only be used for integration testing.
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="connection"></param>
        public WeeeContext(IUserContext userContext, IEventDispatcher dispatcher, DbConnection connection)
            : base(connection, false)
        {
            this.userContext = userContext;
            this.dispatcher = dispatcher;
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

            var affectedMemberUploads = RefreshProducersIsCurrentForComplianceYear();

            int result;
            using (var transaction = Database.BeginTransaction())
            {
                result = base.SaveChanges();

                foreach (var memberUpload in affectedMemberUploads)
                {
                    Database.ExecuteSqlCommand("[Producer].[sppRefreshProducerIsCurrent] @SchemeId = {0}, @ComplianceYear = {1}",
                       memberUpload.SchemeId,
                       memberUpload.ComplianceYear);
                }

                Task.Run(() => this.DispatchEvents(dispatcher)).Wait();

                transaction.Commit();
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.SetEntityId();
            //this.DeleteRemovedRelationships();
            this.AuditChanges(userContext.UserId);

            var affectedMemberUploads = RefreshProducersIsCurrentForComplianceYear();

            int result;
            using (var transaction = Database.BeginTransaction())
            {
                result = await base.SaveChangesAsync(cancellationToken);

                foreach (var memberUpload in affectedMemberUploads)
                {
                    await Database.ExecuteSqlCommandAsync("[Producer].[sppRefreshProducerIsCurrent] @SchemeId = {0}, @ComplianceYear = {1}",
                        cancellationToken,
                        memberUpload.SchemeId,
                        memberUpload.ComplianceYear);
                }

                await this.DispatchEvents(dispatcher);

                transaction.Commit();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the current set of changes being tracked by the context
        /// require that the [IsCurrentForComplianceYear] column needs to be refreshed
        /// for all producers.
        /// 
        /// This will return the affected member uploads if any rows are added, modified
        /// or deleted from the [Producer] table, or if any entry in the [MemberUpload]
        /// table has been updated.
        /// </summary>
        /// <returns></returns>
        private List<MemberUpload> RefreshProducersIsCurrentForComplianceYear()
        {
            var equalityComparer = new MemberUploadRefreshEquality();

            var producerUpdatesMemberUploads = ChangeTracker.Entries()
                .Where(e => e.Entity is Producer)
                .Where(e => (e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted))
                .Select(e => ((Producer)e.Entity).MemberUpload)
                .Distinct(equalityComparer);

            var anyMemberUploadUpdates = ChangeTracker.Entries()
                .Where(e => e.Entity is MemberUpload)
                .Where(e => e.State == EntityState.Modified)
                .Select(e => (MemberUpload)e.Entity)
                .Distinct(equalityComparer);

            return producerUpdatesMemberUploads
                   .Union(anyMemberUploadUpdates, equalityComparer)
                   .ToList();
        }

        public void DeleteOnCommit(Entity entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }

        private class MemberUploadRefreshEquality : IEqualityComparer<MemberUpload>
        {
            public bool Equals(MemberUpload x, MemberUpload y)
            {
                return x.ComplianceYear == y.ComplianceYear &&
                       x.SchemeId == y.SchemeId;
            }

            public int GetHashCode(MemberUpload obj)
            {
                return obj.SchemeId.GetHashCode() ^ obj.ComplianceYear.GetHashCode();
            }
        }
    }
}