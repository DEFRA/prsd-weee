namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class AatfMapping : EntityTypeConfiguration<Aatf>
    {
        public AatfMapping()
        {
            ToTable("AATF", "AATF");

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.ApprovalNumber).HasColumnName("ApprovalNumber").IsRequired().HasMaxLength(20);

            HasRequired<Organisation>(a => a.Organisation);
            HasRequired<AatfAddress>(a => a.SiteAddress);
        }
    }
}
