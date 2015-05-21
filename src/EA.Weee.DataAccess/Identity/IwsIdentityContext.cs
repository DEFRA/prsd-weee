namespace EA.Weee.DataAccess.Identity
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class IwsIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IwsIdentityContext()
            : base("name=Iws.DefaultConnection")
        {
            Database.SetInitializer<IwsIdentityContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Identity");
        }
    }
}