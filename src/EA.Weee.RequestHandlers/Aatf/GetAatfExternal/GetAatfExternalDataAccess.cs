namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Aatf.GetAatfExternal;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class GetAatfExternalDataAccess : IGetAatfExternalDataAccess
    {
        private readonly WeeeContext context;

        public GetAatfExternalDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Aatf> GetAatfById(Guid id)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
