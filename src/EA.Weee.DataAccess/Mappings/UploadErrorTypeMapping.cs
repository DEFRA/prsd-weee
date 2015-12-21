namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class UploadErrorTypeMapping : ComplexTypeConfiguration<UploadErrorType>
    {
        public UploadErrorTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorType");
        }
    }
}