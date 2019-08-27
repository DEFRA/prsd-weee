namespace EA.Weee.DataAccess.Mappings
{
    using Domain.User;
    using System.Data.Entity.ModelConfiguration;

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
