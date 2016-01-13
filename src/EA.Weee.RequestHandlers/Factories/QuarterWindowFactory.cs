namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.DataReturns;

    public class QuarterWindowFactory : IQuarterWindowFactory
    {
        private readonly IQuarterWindowTemplateDataAccess dataAccess;

        public QuarterWindowFactory(IQuarterWindowTemplateDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<QuarterWindow> GetQuarterWindow(Quarter quarter)
        {
            // Otherwise calculate submission window
            var quarterWindowTemplate = await dataAccess.GetByQuarter((int)quarter.Q);

            var startDate = new DateTime(quarter.Year + quarterWindowTemplate.AddStartYears, quarterWindowTemplate.StartMonth, quarterWindowTemplate.StartDay);
            var endDate = new DateTime(quarter.Year + quarterWindowTemplate.AddEndYears, quarterWindowTemplate.EndMonth, quarterWindowTemplate.EndDay);

            return new QuarterWindow(startDate, endDate);
        }
    }
}
