namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;

    internal class WeeeCollectedReturnVersionMapping : EntityTypeConfiguration<WeeeCollectedReturnVersion>
    {
        public WeeeCollectedReturnVersionMapping()
        {
            ToTable("WeeeCollectedReturnVersion", "PCS");

            HasMany(r => r.DataReturnVersions)
                .WithRequired(e => e.WeeeCollectedReturnVersion);

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
