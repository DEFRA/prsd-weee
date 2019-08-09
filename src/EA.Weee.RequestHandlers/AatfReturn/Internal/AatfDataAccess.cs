namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Organisation;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;

    public class AatfDataAccess : IAatfDataAccess
    {
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;

        public AatfDataAccess(WeeeContext context, 
            IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<Aatf> GetDetails(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task UpdateDetails(Aatf oldDetails, Aatf newDetails)
        {
            oldDetails.UpdateDetails(
                newDetails.Name,
                newDetails.CompetentAuthority,
                newDetails.ApprovalNumber,
                newDetails.AatfStatus,
                newDetails.Organisation,
                newDetails.Size,
                newDetails.ApprovalDate,
                newDetails.LocalArea,
                newDetails.PanArea);

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

        public async Task<bool> HasAatfData(Guid aatfId)
        {
            return await context.WeeeSentOn.AnyAsync(p => p.AatfId == aatfId)
                   || await context.WeeeReused.AnyAsync(p => p.AatfId == aatfId)
                   || await context.WeeeReceived.AnyAsync(p => p.AatfId == aatfId)
                    || await context.ReturnAatfs.AnyAsync(r => r.Aatf.Id == aatfId && r.Return.NilReturn);
        }

        public async Task<bool> HasAatfOrganisationOtherAeOrAatf(Aatf aatf)
        {
            var findAatf = await GetAatfById(aatf.Id);

            var organisationId = aatf.Organisation.Id;

            return await context.Aatfs.CountAsync(p => p.Organisation.Id == organisationId 
                                                       && p.ComplianceYear == findAatf.ComplianceYear 
                                                       && p.FacilityType.Value == findAatf.FacilityType.Value
                                                       && p.Id != findAatf.Id) > 0;
        }

        private async Task<Aatf> GetAatfById(Guid id)
        {
            var aatf = await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
            
            if (aatf == null)
            {
                throw new ArgumentException($"Aatf with id {id} not found");
            }

            return aatf;
        }

        public async Task RemoveAatf(Guid aatfId)
        {
            var aatf = await GetAatfById(aatfId);

            genericDataAccess.Remove(aatf);

            foreach (var returnAatf in context.ReturnAatfs.Where(r => r.Aatf.Id == aatfId))
            {
                genericDataAccess.Remove(returnAatf);
            }

            await context.SaveChangesAsync();
        }
    }
}
