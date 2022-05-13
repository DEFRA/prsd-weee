namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class AatfSiteDataAccess : IAatfSiteDataAccess
    {
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;

        public AatfSiteDataAccess(WeeeContext context, IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.genericDataAccess = genericDataAccess;
        }

        public Task Submit(WeeeReusedSite weeeReusedSite)
        {
            context.WeeeReusedSite.Add(weeeReusedSite);

            return context.SaveChangesAsync();
        }

        public async Task<List<AatfAddress>> GetAddresses(Guid aatfId, Guid returnId)
        {
            var weeeReusedId = (await genericDataAccess.GetManyByExpression<WeeeReused>(new WeeeReusedByAatfIdAndReturnIdSpecification(aatfId, returnId))).Last().Id;

            return await context.WeeeReusedSite.Where(w => w.WeeeReused.Id == weeeReusedId).Select(w => w.Address).ToListAsync();
        }

        public async Task<List<WeeeReusedAmount>> GetObligatedWeeeForReturnAndAatf(Guid aatfId, Guid returnId)
        {
            return await context.WeeeReusedAmount.Where(a => a.WeeeReused.ReturnId == returnId && a.WeeeReused.Aatf.Id == aatfId)
                .Include(a => a.WeeeReused.Aatf)
                .Select(a => a).ToListAsync();
        }

        public Task Update(AatfAddress oldAddress, AddressData newAddress, Country country)
        {
            oldAddress.UpdateAddress(
                newAddress.Name,
                newAddress.Address1,
                newAddress.Address2,
                newAddress.TownOrCity,
                newAddress.CountyOrRegion,
                newAddress.Postcode,
                country);

            return context.SaveChangesAsync();
        }
    }
}
