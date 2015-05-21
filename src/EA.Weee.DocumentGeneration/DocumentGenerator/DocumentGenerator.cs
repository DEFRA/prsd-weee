namespace EA.Weee.DocumentGeneration.DocumentGenerator
{
    using System.IO;
    using DocumentFormat.OpenXml.Packaging;
    using Domain;
    using Domain.Notification;

    public class DocumentGenerator : IDocumentGenerator
    {
        private readonly NotificationDocumentMerger notificationDocumentMerger;

        public DocumentGenerator(NotificationDocumentMerger notificationDocumentMerger)
        {
            this.notificationDocumentMerger = notificationDocumentMerger;
        }

        public byte[] GenerateNotificationDocument(NotificationApplication notification, string applicationDirectory)
        {
            return GenerateMainDocument(notification, applicationDirectory);
        }

        private byte[] GenerateMainDocument(NotificationApplication notification, string applicationDirectory)
        {
            var pathToTemplate = Path.Combine(applicationDirectory, "NotificationMergeTemplate.docx");

            // Minimise time the process is using the template file to prevent contention between processes.
            var templateFile = DocumentHelper.ReadDocumentShared(pathToTemplate);

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(templateFile, 0, templateFile.Length);

                using (var document = WordprocessingDocument.Open(memoryStream, true))
                {
                    var mergeFields = MergeFieldLocator.GetMergeRuns(document);

                    notificationDocumentMerger.MergeDataIntoDocument(mergeFields, notification);

                    MergeFieldLocator.RemoveDataSourceSettingFromMergedDocument(document);
                }

                return memoryStream.ToArray();
            }
        }
    }
}