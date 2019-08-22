namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Charges;
    using System.Data.Entity.ModelConfiguration;

    public class IbisFileDataMapping : EntityTypeConfiguration<IbisFileData>
    {
        public IbisFileDataMapping()
        {
            ToTable("IbisFileData", "Charging");

            Property(e => e.FileIdDatabaseValue).HasColumnName("FileId");
        }
    }
}
