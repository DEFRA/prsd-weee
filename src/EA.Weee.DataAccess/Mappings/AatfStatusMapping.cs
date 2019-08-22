namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfStatusMapping : ComplexTypeConfiguration<AatfStatus>
    {
        public AatfStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
