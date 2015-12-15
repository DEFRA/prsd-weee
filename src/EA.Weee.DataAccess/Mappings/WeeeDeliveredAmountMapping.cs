namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.DataReturns;
    internal class WeeeDeliveredAmountMapping : EntityTypeConfiguration<WeeeDeliveredAmount>
    {
        public WeeeDeliveredAmountMapping()
        {
            ToTable("WeeeDeliveredAmount", "PCS");
            Ignore(w => w.DataReturnVersion);
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
