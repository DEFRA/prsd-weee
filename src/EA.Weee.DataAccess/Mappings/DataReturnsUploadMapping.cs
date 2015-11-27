namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnsUploadMapping : EntityTypeConfiguration<DataReturnsUpload>
    {
        public DataReturnsUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("DataReturns", "PCS");
        }
    }
}
