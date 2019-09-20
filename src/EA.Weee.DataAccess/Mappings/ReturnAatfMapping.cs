namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

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
