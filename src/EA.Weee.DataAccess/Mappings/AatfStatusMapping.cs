namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;
    using Domain.User;

    internal class AatfStatusMapping : ComplexTypeConfiguration<AatfStatus>
    {
        public AatfStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
