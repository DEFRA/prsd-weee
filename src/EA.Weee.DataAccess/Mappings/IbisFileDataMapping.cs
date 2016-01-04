namespace EA.Weee.DataAccess.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;

    public class IbisFileDataMapping : EntityTypeConfiguration<IbisFileData>
    {
        public IbisFileDataMapping()
        {
            ToTable("IbisFileData", "Charging");

            Property(e => e.FileIdDatabaseValue).HasColumnName("FileId");
        }
    }
}
