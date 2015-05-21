namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Notification;

    internal class NotificationTypeMapping : ComplexTypeConfiguration<NotificationType>
    {
        public NotificationTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value)
                .HasColumnName("NotificationType");
        }
    }
}