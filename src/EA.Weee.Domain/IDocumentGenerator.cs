namespace EA.Weee.Domain
{
    using Notification;

    public interface IDocumentGenerator
    {
        byte[] GenerateNotificationDocument(NotificationApplication notification, string applicationDirectory);
    }
}