namespace EA.Weee.RequestHandlers.Notification
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Notification;
    using Requests.Shared;

    public class GetExporterByNotificationIdHandler : IRequestHandler<GetExporterByNotificationId, ExporterData>
    {
        private readonly IwsContext context;

        public GetExporterByNotificationIdHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<ExporterData> HandleAsync(GetExporterByNotificationId message)
        {
            return await context.NotificationApplications.Where(n => n.Id == message.NotificationId).Select(n =>
            new ExporterData
            {
                NotificationId = message.NotificationId,
                Name = n.Exporter.Business.Name,
                Type = n.Exporter.Business.Type,
                RegistrationNumber = n.Exporter.Business.RegistrationNumber,
                AdditionalRegistrationNumber = n.Exporter.Business.AdditionalRegistrationNumber,
                Address = new AddressData
                {
                    Building = n.Exporter.Address.Building,
                    StreetOrSuburb = n.Exporter.Address.Address1,
                    Address2 = n.Exporter.Address.Address2,
                    TownOrCity = n.Exporter.Address.TownOrCity,
                    PostalCode = n.Exporter.Address.PostalCode,
                    CountryName = n.Exporter.Address.Country,
                },
                Contact = new ContactData
                {
                    FirstName = n.Exporter.Contact.FirstName,
                    LastName = n.Exporter.Contact.LastName,
                    Telephone = n.Exporter.Contact.Telephone,
                    Fax = n.Exporter.Contact.Fax,
                    Email = n.Exporter.Contact.Email
                }
            }).SingleAsync();
        }
    }
}
