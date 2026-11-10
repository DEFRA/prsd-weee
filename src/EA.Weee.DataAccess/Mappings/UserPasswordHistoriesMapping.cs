namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.User;
    using System.Data.Entity.ModelConfiguration;

    internal class UserPasswordHistoriesMapping : EntityTypeConfiguration<UserPasswordHistories>
    {
        public UserPasswordHistoriesMapping()
        {
            ToTable("UserPasswordHistories", "Identity");

            Property(x => x.Id).HasColumnName("Id").IsRequired();
            Property(x => x.UserId).HasColumnName("UserId").IsRequired().HasMaxLength(128);
            Property(x => x.PasswordHash).HasColumnName("PasswordHash").IsRequired();
            Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        }
    }
}
