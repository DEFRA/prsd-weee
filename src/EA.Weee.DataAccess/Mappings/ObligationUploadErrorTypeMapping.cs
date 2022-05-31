namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Error;
    using Domain.Evidence;

    internal class ObligationUploadErrorTypeMapping : ComplexTypeConfiguration<ObligationUploadErrorType>
    {
        public ObligationUploadErrorTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorType");
        }
    }
}
