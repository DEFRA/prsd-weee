namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.Charges;

    public class IbisFileDataMapping : EntityTypeConfiguration<IbisFileData>
    {
        public IbisFileDataMapping()
        {
            ToTable("IbisFileData", "Charging");

            Property(e => e.FileIdDatabaseValue).HasColumnName("FileId");
        }
    }
}
