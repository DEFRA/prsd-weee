namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ReturnMapping : EntityTypeConfiguration<Return>
    {
        public ReturnMapping()
        {
            ToTable("Return", "AATF");

            Property(dr => dr.Quarter.Year).HasColumnName("ComplianceYear");
            Property(dr => dr.Quarter.Q).HasColumnName("Quarter");
        }
    }
}
