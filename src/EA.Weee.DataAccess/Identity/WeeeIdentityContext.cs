namespace EA.Weee.DataAccess.Identity
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;

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