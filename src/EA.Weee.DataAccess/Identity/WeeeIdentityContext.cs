namespace EA.Weee.DataAccess.Identity
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;

    public class WeeeIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public WeeeIdentityContext()
            : base("name=Weee.DefaultConnection")
        {
            Database.SetInitializer<WeeeIdentityContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Identity");
        }
    }
}