namespace EA.Iws.RequestHandlers.Notification
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Notification;
    using Requests.Shared;

    public class GetProducersByNotificationIdHandler : IRequestHandler<GetProducersByNotificationId, IList<ProducerData>>
    {
        private readonly IwsContext db;

        public GetProducersByNotificationIdHandler(IwsContext db)
        {
            this.db = db;
        }

        public async Task<IList<ProducerData>> HandleAsync(GetProducersByNotificationId message)
        {
            var result = await db.NotificationApplications.Where(n => n.Id == message.NotificationId).SingleAsync();
                        
            var producers = result.Producers.Select(p => new ProducerData
            {
                ProducerId = p.Id,
                Name = p.Business.Name,
                Address =
                    new AddressData
                    {
                        Address2 = p.Address.Address2,
                        CountryName = p.Address.Country,
                        Building = p.Address.Building,
                        PostalCode = p.Address.PostalCode,
                        StreetOrSuburb = p.Address.Address1,
                        TownOrCity = p.Address.TownOrCity
                    },
                CompaniesHouseNumber = p.Business.RegistrationNumber,
                IsSiteOfExport = p.IsSiteOfExport,
                Contact = new ContactData
                {
                    Email = p.Contact.Email,
                    FirstName = p.Contact.FirstName,
                    LastName = p.Contact.LastName,
                    Fax = p.Contact.Fax,
                    Telephone = p.Contact.Telephone
                },
                AdditionalRegistrationNumber = p.Business.AdditionalRegistrationNumber,
                NotificationId = message.NotificationId
            }).ToList();

            return producers;
        }
    }
}
