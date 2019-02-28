namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class AddAatfSiteDataAccess : IAddAatfSiteDataAccess
    {
        private readonly WeeeContext context;

        public AddAatfSiteDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(WeeeReusedSite weeeReusedSite)
        {
            context.WeeeReusedSite.Add(weeeReusedSite);

            return context.SaveChangesAsync();
        }
    }
}
