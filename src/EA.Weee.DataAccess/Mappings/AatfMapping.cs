namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using Domain.Lookup;
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfMapping : EntityTypeConfiguration<Aatf>
    {
        public AatfMapping()
        {
            ToTable("AATF", "AATF");

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.ApprovalNumber).HasColumnName("ApprovalNumber").IsRequired().HasMaxLength(20);

            HasRequired<Organisation>(a => a.Organisation);
            HasRequired<AatfAddress>(a => a.SiteAddress);

            HasOptional<LocalArea>(x => x.LocalArea);
            HasOptional<PanArea>(x => x.PanArea);
        }
    }
}
