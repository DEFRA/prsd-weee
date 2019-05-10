namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;

    public class AatfContactDataAccess : IAatfContactDataAccess
    {
        private readonly WeeeContext context;

        public AatfContactDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<AatfContact> GetContact(Guid aatfId)
        {
            var result = await context.AatfContacts.SingleOrDefaultAsync(a => a.Id == (context.Aatfs.FirstOrDefault(c => c.Id == aatfId)).Contact.Id);

            return result;
        }

        public Task Update(AatfContact oldDetails, AatfContactData newDetails, Country country)
        {
            oldDetails.UpdateDetails(
                newDetails.FirstName,
                newDetails.LastName,
                newDetails.Position,
                newDetails.AddressData.Address1,
                newDetails.AddressData.Address2,
                newDetails.AddressData.TownOrCity,
                newDetails.AddressData.CountyOrRegion,
                newDetails.AddressData.Postcode,
                country,
                newDetails.Telephone,
                newDetails.Email);

            return context.SaveChangesAsync();
        }
    }
}
