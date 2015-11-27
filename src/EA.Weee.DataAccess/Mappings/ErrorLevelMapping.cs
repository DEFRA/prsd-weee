namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain;
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