namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;
    internal class WeeeCollectedAmountMapping : EntityTypeConfiguration<WeeeCollectedAmount>
    {
        public WeeeCollectedAmountMapping()
        {
            ToTable("WeeeCollectedAmount", "PCS");
            HasMany(w => w.WeeeCollectedReturnVersions)                
                .WithMany(a => a.WeeeCollectedAmounts)
                .Map(
               m =>
               {
                   m.MapLeftKey("WeeeCollectedReturnVersionId");
                   m.MapRightKey("WeeCollectedAmountId");
                   m.ToTable("WeeeCollectedReturnVersionAmount");
               });
        }
    }
}
