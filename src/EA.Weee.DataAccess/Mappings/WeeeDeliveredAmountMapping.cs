namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;

    internal class WeeeDeliveredAmountMapping : EntityTypeConfiguration<WeeeDeliveredAmount>
    {
        public WeeeDeliveredAmountMapping()
        {
            ToTable("WeeeDeliveredAmount", "PCS");

            HasMany(m => m.WeeeDeliveredReturnVersions)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("WeeeDeliveredAmountReturnVersionId");
                    m.MapRightKey("WeeeDeliveredAmountAmountId");
                    m.ToTable("WeeeDeliveredAmountReturnVersionAmount");
                });

            Map<AatfDeliveredAmount>(m =>
            {
                m.MapInheritedProperties();
                m.Requires(p => p.AatfDeliveryLocation);
                m.ToTable("WeeeDeliveredAmount", "PCS");
            });

            Map<AeDeliveredAmount>(m =>
            {
                m.MapInheritedProperties();
                m.Requires(p => p.AeDeliveryLocation);
                m.ToTable("WeeeDeliveredAmount", "PCS");
            });
        }
    }
}
