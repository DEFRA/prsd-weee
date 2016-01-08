namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Lookup;

    internal class QuarterWindowTemplateMapping : EntityTypeConfiguration<QuarterWindowTemplate>
    {
        public QuarterWindowTemplateMapping()
        {
            ToTable("QuarterWindowTemplate", "Lookup");
        }
    }
}
