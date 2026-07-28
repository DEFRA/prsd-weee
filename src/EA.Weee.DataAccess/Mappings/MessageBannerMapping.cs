namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;

    internal class MessageBannerMapping : EntityTypeConfiguration<MessageBanner>
    {
        public MessageBannerMapping()
        {
            ToTable("MessageBanner", "Lookup");
        }
    }
}
