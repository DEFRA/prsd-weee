namespace EA.Weee.DataAccess.DataAccess
{
    using System.Linq;
    using Domain.Lookup;

    public class QuarterWindowTemplateDataAccess : IQuarterWindowTemplateDataAccess
    {
        private readonly WeeeContext context;

        public QuarterWindowTemplateDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public QuarterWindowTemplate GetByQuarter(int quarter)
        {
            return context.QuarterWindows.Single(qw => qw.Quarter == quarter);
        }
    }
}
