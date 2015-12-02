namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnsUploadMapping : EntityTypeConfiguration<DataReturnsUpload>
    {
        public DataReturnsUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("DataReturnsUpload", "PCS");
        }
    }
}
