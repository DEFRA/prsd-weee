﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfMapping : EntityTypeConfiguration<Aatf>
    {
        public AatfMapping()
        {
            ToTable("AATF", "AATF");

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.ApprovalNumber).HasColumnName("ApprovalNumber").IsRequired().HasMaxLength(20);

            HasRequired<Operator>(a => a.Operator);
            HasRequired<AatfAddress>(a => a.SiteAddress);
        }
    }
}
