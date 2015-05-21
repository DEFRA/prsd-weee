namespace EA.Weee.RequestHandlers.Facilities
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mediator;
    using Requests.Facilities;
    using Requests.Shared;

    public class AddFacilityToNotificationHandler : IRequestHandler<AddFacilityToNotification, Guid>
    {
        private readonly IwsContext context;

        public AddFacilityToNotificationHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(AddFacilityToNotification message)
        {
            var notification = await context.NotificationApplications.FindAsync(message.Facility.NotificationId);

            if (notification == null)
            {
                throw new InvalidOperationException("Attempted to add a facility to a missing notification");
            }

            var country = await context.Countries.SingleAsync(c => c.Id == message.Facility.Address.CountryId);

            var address = CreateAddress(message.Facility.Address, country.Name);

            var contact = CreateContact(message.Facility.Contact);

            var business = CreateBusiness(message.Facility.Business);

            var facility = new Facility(business, address, contact, country, message.Facility.IsActualSiteOfTreatment);

            notification.AddFacility(facility);

            await context.SaveChangesAsync();

            return facility.Id;
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
