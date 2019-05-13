namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;

    internal class AatfSizeMapping : ComplexTypeConfiguration<AatfSize>
    {
        public AatfSizeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Size");
        }
    }
}
