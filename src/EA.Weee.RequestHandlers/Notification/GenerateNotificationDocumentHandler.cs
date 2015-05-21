namespace EA.Weee.RequestHandlers.Notification
{
    using System;
    using System.Data.Entity;
    using System.IO;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Notification;

    public class GenerateNotificationDocumentHandler : IRequestHandler<GenerateNotificationDocument, byte[]>
    {
        private readonly IwsContext context;
        private readonly IDocumentGenerator documentGenerator;
        private readonly IUserContext userContext;

        public GenerateNotificationDocumentHandler(IwsContext context, 
            IDocumentGenerator documentGenerator,
            IUserContext userContext)
        {
            this.context = context;
            this.documentGenerator = documentGenerator;
            this.userContext = userContext;
        }

        public async Task<byte[]> HandleAsync(GenerateNotificationDocument query)
        {
            Guid userId = userContext.UserId;

            var notification = await context
                .NotificationApplications
                .SingleAsync(n => n.Id == query.NotificationId 
                    && n.UserId == userId);

            return documentGenerator.GenerateNotificationDocument(notification, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents"));
        }
    }
}