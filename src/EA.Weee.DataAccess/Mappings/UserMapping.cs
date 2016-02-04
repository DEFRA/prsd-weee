namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.User;

    internal class UserMapping : EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            ToTable("AspNetUsers", "Identity")
                .Property(p => p.Id)
                .HasColumnName("Id");

            Ignore(u => u.FullName);
        }
    }
}
