namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class ReturnStatusMapping : ComplexTypeConfiguration<ReturnStatus>
    {
        public ReturnStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ReturnStatus");
        }
    }
}
