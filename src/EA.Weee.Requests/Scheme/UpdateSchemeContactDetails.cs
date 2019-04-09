namespace EA.Weee.Requests.Scheme
{
    using Core.Organisations;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class UpdateSchemeContactDetails : IRequest<bool>
    {
        public SchemeData SchemeData { get; private set; }

        public bool SendNotificationOnChange { get; set; }

        public UpdateSchemeContactDetails(SchemeData schemeData, bool sendNotificationOnChange = false)
        {
            SchemeData = schemeData;
            SendNotificationOnChange = sendNotificationOnChange;
        }
    }
}
