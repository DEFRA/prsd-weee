namespace EA.Weee.Domain.Notification
{
    using Prsd.Core.Domain;

    public class NotificationType : Enumeration
    {
        public static readonly NotificationType Recovery = new NotificationType(1, "Recovery");
        public static readonly NotificationType Disposal = new NotificationType(2, "Disposal");

        private NotificationType()
        {
        }

        private NotificationType(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}