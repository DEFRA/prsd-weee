namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfSizeMapping : ComplexTypeConfiguration<AatfSize>
    {
        public AatfSizeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Size");
        }
    }
}
