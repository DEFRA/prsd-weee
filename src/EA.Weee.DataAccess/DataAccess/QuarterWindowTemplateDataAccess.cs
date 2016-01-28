namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Lookup;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class QuarterWindowTemplateDataAccess : IQuarterWindowTemplateDataAccess
    {
        private readonly WeeeContext context;

        public QuarterWindowTemplateDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<QuarterWindowTemplate> GetByQuarter(int quarter)
        {
            return await context.QuarterWindowTemplates.SingleAsync(qw => qw.Quarter == quarter);
        }

        public async Task<List<QuarterWindowTemplate>> GetAll()
        {
            return await context.QuarterWindowTemplates.ToListAsync();
        }
    }
}
