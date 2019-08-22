namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    public class EeeOutputReturnVersionMapping : EntityTypeConfiguration<EeeOutputReturnVersion>
    {
        public EeeOutputReturnVersionMapping()
        {
            ToTable("EeeOutputReturnVersion", "PCS");

            HasMany(r => r.DataReturnVersions)
                .WithOptional(e => e.EeeOutputReturnVersion);

            HasMany(e => e.EeeOutputAmounts)
                .WithMany(r => r.EeeOutputReturnVersions)
                .Map(m =>
                {
                    m.MapLeftKey("EeeOutputReturnVersionId");
                    m.MapRightKey("EeeOuputAmountId");
                    m.ToTable("EeeOutputReturnVersionAmount", "PCS");
                });
        }
    }
}
