namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;

    public class AddOffSiteDataAccess : IAddOffSiteDataAccess
    {
        private readonly WeeeContext context;

        public AddOffSiteDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(Address address)
        {
            context.Address.Add(address);

            return context.SaveChangesAsync();
        }
    }
}
