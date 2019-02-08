namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeReceivedMapping : EntityTypeConfiguration<AatfWeeReceived>
    {
        public AatfWeeReceivedMapping()
        {
            ToTable("AatfWeeReceived", "AATF");
        }
    }
}
