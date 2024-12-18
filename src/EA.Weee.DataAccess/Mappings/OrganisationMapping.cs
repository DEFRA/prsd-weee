namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationMapping : EntityTypeConfiguration<Organisation>
    {
        public OrganisationMapping()
        {
            ToTable("Organisation", "Organisation");
            Property(o => o.NpwdMigrated).IsRequired().HasColumnName("NPWDMigrated");
            Property(o => o.NpwdMigratedComplete).IsRequired().HasColumnName("NPWDMigratedComplete");
            HasMany(o => o.Schemes).WithRequired(o => o.Organisation).HasForeignKey(o => o.OrganisationId);

            HasOptional(p => p.ProducerBalancingScheme)
                .WithOptionalPrincipal(o => o.Organisation)
                .Map(x => x.MapKey("OrganisationId"));
        }
    }
}
