namespace EA.Weee.RequestHandlers.Notification
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Notification;
    using Prsd.Core.Mediator;
    using Requests.Notification;

    internal class CreateImporterHandler : IRequestHandler<CreateImporter, Guid>
    {
        private readonly IwsContext context;

        public CreateImporterHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<Guid> HandleAsync(CreateImporter command)
        {
            var country = await context.Countries.SingleAsync(c => c.Id == command.CountryId);

            var address = new Address(command.Building,
                command.Address1,
                command.Address2, command.TownOrCity, command.PostalCode, country.Name);

            var contact = new Contact(command.FirstName,
                command.LastName,
                command.Phone,
                command.Email,
                command.Fax);

            var business = new Business(command.Name,
                command.Type,
                command.RegistrationNumber,
                command.AdditionalRegistrationNumber);

            var importer = new Importer(business, address, contact);

            var notification = await context.NotificationApplications.FindAsync(command.NotificationId);
            notification.AddImporter(importer);

            await context.SaveChangesAsync();

            return importer.Id;
        }
    }
}
