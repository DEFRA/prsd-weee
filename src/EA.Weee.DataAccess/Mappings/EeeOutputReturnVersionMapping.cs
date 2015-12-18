namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    public class EeeOutputReturnVersionMapping : EntityTypeConfiguration<EeeOutputReturnVersion>
    {
        public EeeOutputReturnVersionMapping()
        {
            ToTable("EeeOutputReturnVersion", "PCS");
            HasMany(r => r.DataReturnVersions);
            HasMany(e => e.EeeOutputAmounts);
        }
    }
}
