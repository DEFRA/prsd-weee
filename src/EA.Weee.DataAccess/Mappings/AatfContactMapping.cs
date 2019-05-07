namespace EA.Weee.DataAccess.Mappings
{ 
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfContactMapping : EntityTypeConfiguration<AatfContact>
    {
        public AatfContactMapping()
        {
            ToTable("AatfContact", "AATF");

            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(35);
            Property(x => x.LastName).HasColumnName("LasttName").IsRequired().HasMaxLength(35);
            Property(x => x.Telephone).HasColumnName("Telephone").HasMaxLength(35);
            Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(35);
            Property(x => x.Position).HasColumnName("Position").HasMaxLength(35);
        }
    }
}
