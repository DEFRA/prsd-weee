namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class SchemeStatusMapping : ComplexTypeConfiguration<SchemeStatus>
    {
        public SchemeStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("SchemeStatus");
        }
    }
}
