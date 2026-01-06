namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;

    internal class DirectRegistrantChargeMapping : EntityTypeConfiguration<DirectRegistrantCharge>
    {
        public DirectRegistrantChargeMapping()
        {
            ToTable("DirectRegistrantCharge", "Lookup");
        }
    }
}
