namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Error;
    using System.Data.Entity.ModelConfiguration;

    internal class ObligationUploadErrorTypeMapping : ComplexTypeConfiguration<ObligationUploadErrorType>
    {
        public ObligationUploadErrorTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorType");
        }
    }
}
