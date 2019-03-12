namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeSentOnMapping : EntityTypeConfiguration<WeeeSentOn>
    {
        public AatfWeeeSentOnMapping()
        {
            ToTable("WeeeSentOn", "AATF");
        }
    }
}
