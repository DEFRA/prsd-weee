namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    public class WeeeDeliveredReturnVersionMapping : EntityTypeConfiguration<WeeeDeliveredReturnVersion>
    {
        public WeeeDeliveredReturnVersionMapping()
        {
            ToTable("WeeeDeliveredReturnVersion", "PCS");

            HasMany(r => r.DataReturnVersions);
            HasMany(at => at.AatfDeliveredAmounts);
            HasMany(ae => ae.AatfDeliveredAmounts);
        }
    }
}
