namespace EA.Weee.DataAccess.Mappings
{
    using Domain;
    using System.Data.Entity.ModelConfiguration;

    internal class UploadErrorTypeMapping : ComplexTypeConfiguration<UploadErrorType>
    {
        public UploadErrorTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorType");
        }
    }
}