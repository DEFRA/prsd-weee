namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;

    public class ReturnAatfMapping : EntityTypeConfiguration<ReturnAatf>
    {
        public ReturnAatfMapping()
        {
            ToTable("ReturnAatf", "AATF");

            HasRequired<Aatf>(a => a.Aatf);
            HasRequired<Return>(a => a.Return);
        }
    }
}
