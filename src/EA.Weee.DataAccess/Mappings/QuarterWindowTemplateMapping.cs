namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;

    internal class QuarterWindowTemplateMapping : EntityTypeConfiguration<QuarterWindowTemplate>
    {
        public QuarterWindowTemplateMapping()
        {
            ToTable("QuarterWindowTemplate", "Lookup");
        }
    }
}
