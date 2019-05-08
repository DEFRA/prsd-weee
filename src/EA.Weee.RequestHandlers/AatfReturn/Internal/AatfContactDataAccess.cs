namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
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
            return await context.AatfContacts.SingleOrDefaultAsync(a => a.Id == (context.Aatfs.First(c => c.Id == aatfId)).Contact.Id);
        }
    }
}
