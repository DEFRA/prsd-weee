namespace EA.Weee.RequestHandlers.Producers
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Notification;
    using Prsd.Core.Mediator;
    using Requests.Producers;
    using Requests.Shared;

    internal class AddProducerToNotificationHandler : IRequestHandler<AddProducerToNotification, Guid>
    {
        private readonly IwsContext context;

        public AddProducerToNotificationHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(AddProducerToNotification command)
        {
            var country = await context.Countries.SingleAsync(c => c.Id == command.ProducerData.Address.CountryId);

            var address = CreateAddress(command.ProducerData.Address, country.Name);

            var contact = CreateContact(command.ProducerData.Contact);

            var business = CreateBusiness(command.ProducerData.Business);

            var producer = new Producer(business,
                address,
                contact,
                command.ProducerData.IsSiteOfExport);

            var notification = await context.NotificationApplications.FindAsync(command.ProducerData.NotificationId);
            notification.AddProducer(producer);

            await context.SaveChangesAsync();

            return producer.Id;
        }

        private Business CreateBusiness(BusinessData business)
        {
            return new Business(business.Name, business.EntityType, business.RegistrationNumber, business.AdditionalRegistrationNumber);
        }

        private Contact CreateContact(ContactData contact)
        {
            return new Contact(contact.FirstName, contact.LastName, contact.Telephone, contact.Email, contact.Fax);
        }

        private Address CreateAddress(AddressData address, string countryName)
        {
            return new Address(address.Building,
                address.StreetOrSuburb,
                address.Address2,
                address.TownOrCity,
                address.PostalCode,
                address.CountryName ?? countryName);
        }
    }
}