namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;
    using Domain.User;

    internal class FacilityTypeMapping : ComplexTypeConfiguration<FacilityType>
    {
        public FacilityTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("FacilityType");
        }
    }
}
