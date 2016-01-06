namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Lookup;

    internal class QuarterWindowMapping : EntityTypeConfiguration<QuarterWindow>
    {
        public QuarterWindowMapping()
        {
            ToTable("QuarterWindow", "Lookup");
        }
    }
}
