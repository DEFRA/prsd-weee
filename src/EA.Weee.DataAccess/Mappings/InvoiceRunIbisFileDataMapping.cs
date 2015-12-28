namespace EA.Weee.DataAccess.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;

    public class InvoiceRunIbisFileDataMapping : EntityTypeConfiguration<InvoiceRunIbisFileData>
    {
        public InvoiceRunIbisFileDataMapping()
        {
            ToTable("InvoiceRun", "Charging");

            Property(e => e.FileIdDatabaseValue).HasColumnName("IbisFileId");
            Property(e => e.CustomerFileName).HasColumnName("IbisCustomerFileName");
            Property(e => e.CustomerFileData).HasColumnName("IbisCustomerFileData");
            Property(e => e.TransactionFileName).HasColumnName("IbisTransactionFileName");
            Property(e => e.TransactionFileData).HasColumnName("IbisTransactionFileData");
        }
    }
}
