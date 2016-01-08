namespace EA.Weee.DataAccess.DataAccess
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Domain.Lookup;

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
    }
}
