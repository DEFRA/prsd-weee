namespace EA.Weee.DataAccess.DataAccess
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Domain;

    public class SystemDataDataAccess : ISystemDataDataAccess
    {
        private readonly WeeeContext context;

        public SystemDataDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<SystemData> Get()
        {
            return await context.SystemData.SingleAsync();
        }
    }
}
