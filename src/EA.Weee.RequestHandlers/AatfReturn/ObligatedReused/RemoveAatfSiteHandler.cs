namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;

    public class RemoveAatfSiteHandler : IRequestHandler<RemoveAatfSite, bool>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfSiteDataAccess aatfSiteDataAccess;

        public RemoveAatfSiteHandler(WeeeContext context, IWeeeAuthorization authorization, IGenericDataAccess genericDataAccess, IAatfSiteDataAccess aatfSiteDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfSiteDataAccess = aatfSiteDataAccess;
        }

        public async Task<bool> HandleAsync(RemoveAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var site = await genericDataAccess.GetById<AatfAddress>(message.SiteId);
            var weeeReusedSiteId = (await genericDataAccess.GetSingleByExpression<WeeeReusedSite>(new WeeeReusedByAddressIdSpecification(message.SiteId))).Id;
            var weeeReusedSite = await genericDataAccess.GetById<WeeeReusedSite>(weeeReusedSiteId);

            genericDataAccess.Remove(weeeReusedSite);

            genericDataAccess.Remove(site);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
