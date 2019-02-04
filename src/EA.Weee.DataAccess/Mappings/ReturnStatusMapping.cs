namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;
    using Domain.User;

    internal class ReturnStatusMapping : ComplexTypeConfiguration<ReturnStatus>
    {
        public ReturnStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ReturnStatus");
        }
    }
}
