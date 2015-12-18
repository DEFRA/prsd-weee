namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    public class EeeOutputAmountMapping : EntityTypeConfiguration<EeeOutputAmount>
    {
        public EeeOutputAmountMapping()
        {
            ToTable("EeeOutputAmount", "PCS");
            HasMany(w => w.EeeOutputReturnVersions)
                .WithMany(a => a.EeeOutputAmounts)
                .Map(
               m =>
               {
                   m.MapLeftKey("EeeOutputAmountReturnVersionId");
                   m.MapRightKey("EeeOutputAmountAmountId");
                   m.ToTable("EeeOutputAmountReturnVersionAmount");
               });
        }
    }
}
