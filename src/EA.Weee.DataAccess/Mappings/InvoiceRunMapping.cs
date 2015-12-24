namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Charges;

    internal class InvoiceRunMapping : EntityTypeConfiguration<InvoiceRun>
    {
        public InvoiceRunMapping()
        {
            ToTable("InvoiceRun", "Charging");
        }
    }
}
