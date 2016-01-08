namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    public class WeeeDeliveredReturnVersionMapping : EntityTypeConfiguration<WeeeDeliveredReturnVersion>
    {
        public WeeeDeliveredReturnVersionMapping()
        {
            ToTable("WeeeDeliveredReturnVersion", "PCS");

            HasMany(r => r.DataReturnVersions)
                .WithRequired(e => e.WeeeDeliveredReturnVersion);

            HasMany(w => w.WeeeDeliveredAmounts)
                .WithMany(r => r.WeeeDeliveredReturnVersions)
                .Map(m =>
                {
                    m.MapLeftKey("WeeeDeliveredReturnVersionId");
                    m.MapRightKey("WeeeDeliveredAmountId");
                    m.ToTable("WeeeDeliveredReturnVersionAmount", "PCS");
                });
        }
    }
}
