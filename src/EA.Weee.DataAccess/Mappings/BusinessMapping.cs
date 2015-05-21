namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class BusinessMapping : ComplexTypeConfiguration<Business>
    {
        public BusinessMapping()
        {
            Property(x => x.Name).HasColumnName("Name");
            Property(x => x.Type).HasColumnName("Type");
            Property(x => x.RegistrationNumber).HasColumnName("RegistrationNumber");
            Property(x => x.AdditionalRegistrationNumber).HasColumnName("AdditionalRegistrationNumber");
        }
    }
}
