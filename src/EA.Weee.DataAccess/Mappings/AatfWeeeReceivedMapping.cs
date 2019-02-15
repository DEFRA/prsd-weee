namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeReceivedMapping : EntityTypeConfiguration<WeeeReceived>
    {
        public AatfWeeeReceivedMapping()
        {
            ToTable("WeeeReceived", "AATF");
        }
    }
}
