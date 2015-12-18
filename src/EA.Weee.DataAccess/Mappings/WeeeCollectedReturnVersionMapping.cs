namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;

    internal class WeeeCollectedReturnVersionMapping : EntityTypeConfiguration<WeeeCollectedReturnVersion>
    {
        public WeeeCollectedReturnVersionMapping()
        {
            ToTable("WeeeCollectedReturnVersion", "PCS");
            HasMany(r => r.DataReturnVersions);
            HasMany(w => w.WeeeCollectedAmounts);
        }
    }
}
