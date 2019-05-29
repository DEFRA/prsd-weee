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

    public class AatfDataAccess : IAatfDataAccess
    {
        private readonly WeeeContext context;

        public AatfDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Aatf> GetDetails(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task UpdateDetails(Aatf oldDetails, Aatf newDetails)
        {
            oldDetails.UpdateDetails(
                newDetails.Name,
                newDetails.CompetentAuthorityId,
                newDetails.ApprovalNumber,
                newDetails.AatfStatus,
                newDetails.Operator,
                newDetails.Size,
                newDetails.ApprovalDate);

            return context.SaveChangesAsync();
        }

        public Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country)
        {
            oldDetails.UpdateAddress(
                newDetails.Name,
                newDetails.Address1,
                newDetails.Address2,
                newDetails.TownOrCity,
                newDetails.CountyOrRegion,
                newDetails.Postcode,
                country);

            return context.SaveChangesAsync();
        }

        public async Task<AatfContact> GetContact(Guid aatfId)
        {
            return await context.AatfContacts.SingleOrDefaultAsync(a => a.Id == (context.Aatfs.FirstOrDefault(c => c.Id == aatfId)).Contact.Id);
        }

        public Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country)
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
