namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class DataReturnsUploadMapping : EntityTypeConfiguration<DataReturnsUpload>
    {
        public DataReturnsUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("DataReturns", "PCS");
        }
    }
}
