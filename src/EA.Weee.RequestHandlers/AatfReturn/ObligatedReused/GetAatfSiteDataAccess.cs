namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;

    internal class GetAatfSiteDataAccess : IGetAatfSiteDataAccess
    {
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;

        public GetAatfSiteDataAccess(WeeeContext context, IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<List<AatfAddress>> GetAddresses(Guid aatfId, Guid returnId)
        {
            var weeeReusedId = (await genericDataAccess.GetManyByExpression<WeeeReused>(new WeeeReusedByAatfIdAndReturnIdSpecification(aatfId, returnId))).Last().Id;

            return await context.WeeeReusedSite.Where(w => w.WeeeReused.Id == weeeReusedId).Select(w => w.Address).ToListAsync();
        }
    }
}
