namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Error;
    using System.Data.Entity.ModelConfiguration;

    internal class ErrorLevelMapping : ComplexTypeConfiguration<ErrorLevel>
    {
        public ErrorLevelMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorLevel");
        }
    }
}