namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class ReturnSchemeDataAccess : IReturnSchemeDataAccess
    {
        private readonly WeeeContext context;

        public ReturnSchemeDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Submit(ReturnScheme scheme)
        {
            context.ReturnScheme.Add(scheme);

            await context.SaveChangesAsync();

            return scheme.Id;
        }

        public async Task<ReturnScheme> GetById(Guid returnSchemeId)
        {
            return await context.ReturnScheme.Include(r => r.SchemeId).Include(r => r.ReturnId).SingleOrDefaultAsync(o => o.Id == returnSchemeId);
        }
    }
}
