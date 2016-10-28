namespace EA.Weee.RequestHandlers.Admin
{
    using System.Threading.Tasks;
    using Email;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class SendTestEmailHandler : IRequestHandler<SendTestEmail, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeEmailService weeeEmailService;

        public SendTestEmailHandler(IWeeeAuthorization authorization, IWeeeEmailService weeeEmailService)
        {
            this.authorization = authorization;
            this.weeeEmailService = weeeEmailService;
        }

        public Task<bool> HandleAsync(SendTestEmail message)
        {
            authorization.EnsureCanAccessInternalArea(false);

            return weeeEmailService.SendTestEmail(message.EmailTo);
        }
    }
}
