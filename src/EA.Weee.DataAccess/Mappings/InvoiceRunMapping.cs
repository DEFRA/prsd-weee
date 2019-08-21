namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Charges;
    using System.Data.Entity.ModelConfiguration;

    internal class InvoiceRunMapping : EntityTypeConfiguration<InvoiceRun>
    {
        public InvoiceRunMapping()
        {
            ToTable("InvoiceRun", "Charging");

            HasRequired(e => e.IssuedByUser);
        }
    }
}
