namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class ErrorLevelMapping : ComplexTypeConfiguration<ErrorLevel>
    {
        public ErrorLevelMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("ErrorLevel");
        }
    }
}