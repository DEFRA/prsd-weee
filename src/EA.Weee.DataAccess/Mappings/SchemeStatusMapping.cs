namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class SchemeStatusMapping : ComplexTypeConfiguration<SchemeStatus>
    {
        public SchemeStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("SchemeStatus");
        }
    }
}
