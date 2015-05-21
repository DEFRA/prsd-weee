namespace EA.Weee.RequestHandlers.Facilities
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Facilities;
    using Requests.Shared;

    public class GetFacilitiesByNotificationIdHandler : IRequestHandler<GetFacilitiesByNotificationId, IList<FacilityData>>
    {
        private readonly IwsContext db;

        public GetFacilitiesByNotificationIdHandler(IwsContext db)
        {
            this.db = db;
        }

        public async Task<IList<FacilityData>> HandleAsync(GetFacilitiesByNotificationId message)
        {
            var result = await db.NotificationApplications.Where(n => n.Id == message.NotificationId).SingleAsync();

            var producers = result.Facilities.Select(p => new FacilityData
            {
                Business =
                    new BusinessData
                    {
                        AdditionalRegistrationNumber = p.Business.AdditionalRegistrationNumber,
                        CompaniesHouseRegistrationNumber = p.Business.RegistrationNumber,
                        EntityType = p.Business.Type,
                        Name = p.Business.Name
                    },
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
                Contact = new ContactData
                {
                    Email = p.Contact.Email,
                    FirstName = p.Contact.FirstName,
                    LastName = p.Contact.LastName,
                    Fax = p.Contact.Fax,
                    Telephone = p.Contact.Telephone
                },
                IsActualSiteOfTreatment = p.IsActualSiteOfTreatment,
                NotificationId = message.NotificationId
            }).ToList();

            return producers;
        }
    }
}