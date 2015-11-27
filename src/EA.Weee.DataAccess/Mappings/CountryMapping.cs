namespace EA.Weee.DataAccess.Mappings
{
    using Domain;
    using System.Data.Entity.ModelConfiguration;
    internal class CountryMapping : EntityTypeConfiguration<Country>
    {
        public CountryMapping()
        {
            this.ToTable("Country", "Lookup");
        }
    }
}