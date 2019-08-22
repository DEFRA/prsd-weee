namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class FacilityTypeMapping : ComplexTypeConfiguration<FacilityType>
    {
        public FacilityTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("FacilityType");
        }
    }
}
