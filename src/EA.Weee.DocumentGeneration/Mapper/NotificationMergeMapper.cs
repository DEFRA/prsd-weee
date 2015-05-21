namespace EA.Weee.DocumentGeneration.Mapper
{
    using System;
    using Domain.Notification;

    internal class NotificationMergeMapper : INotificationMergeMapper
    {
        public string GetValueForMergeField(MergeField mergeField, NotificationApplication notification)
        {
            if (mergeField.FieldName.InnerTypeName.Equals("Number"))
            {
                return notification.NotificationNumber;
            }

            if (notification.NotificationType == NotificationType.Disposal)
            {
                if (mergeField.FieldName.InnerTypeName.Equals("IsDisposal"))
                {
                    return "☑";
                }

                if (mergeField.FieldName.InnerTypeName.Equals("IsRecovery"))
                {
                    return "☐";
                }
            }

            if (notification.NotificationType == NotificationType.Recovery)
            {
                if (mergeField.FieldName.InnerTypeName.Equals("IsRecovery"))
                {
                    return "☑";
                }

                if (mergeField.FieldName.InnerTypeName.Equals("IsDisposal"))
                {
                    return "☐";
                }
            }

            return String.Empty;
        }
    }
}