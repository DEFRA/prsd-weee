namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class WeeeCollectedReturnVersionMapping : EntityTypeConfiguration<WeeeCollectedReturnVersion>
    {
        public WeeeCollectedReturnVersionMapping()
        {
            ToTable("WeeeCollectedReturnVersion", "PCS");

            HasMany(r => r.DataReturnVersions)
                .WithOptional(e => e.WeeeCollectedReturnVersion);

            HasMany(e => e.WeeeCollectedAmounts)
                .WithMany(r => r.WeeeCollectedReturnVersions)
                .Map(m =>
                {
                    m.MapLeftKey("WeeeCollectedReturnVersionId");
                    m.MapRightKey("WeeeCollectedAmountId");
                    m.ToTable("WeeeCollectedReturnVersionAmount", "PCS");
                });
        }
    }
}
