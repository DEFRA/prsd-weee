namespace EA.Weee.DocumentGeneration
{
    using System.Collections.Generic;
    using Domain.Notification;
    using Mapper;

    public class NotificationDocumentMerger
    {
        private readonly INotificationMergeMapper notificationMergeMapper;

        public NotificationDocumentMerger(INotificationMergeMapper notificationMergeMapper)
        {
            this.notificationMergeMapper = notificationMergeMapper;
        }

        internal void MergeDataIntoDocument(IList<MergeField> mergeFields, NotificationApplication notification)
        {
            foreach (var mergeField in mergeFields)
            {
                mergeField.RemoveCurrentContents();

                string value = notificationMergeMapper.GetValueForMergeField(mergeField, notification);

                mergeField.SetText(value);
            }
        }
    }
}