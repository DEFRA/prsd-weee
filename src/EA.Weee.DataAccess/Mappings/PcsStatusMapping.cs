namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class PcsStatusMapping : ComplexTypeConfiguration<PcsStatus>
    {
        public PcsStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("PCSStatus");
        }
    }
}
