namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class ObligationTypeMapping : ComplexTypeConfiguration<ObligationType>
    {
        public ObligationTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ObligationType");
        }
    }
}
