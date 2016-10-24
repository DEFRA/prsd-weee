namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class SendTestEmail : IRequest<bool>
    {
        public SendTestEmail(string emailTo)
        {
            EmailTo = emailTo;
        }

        public string EmailTo { get; private set; }
    }
}
