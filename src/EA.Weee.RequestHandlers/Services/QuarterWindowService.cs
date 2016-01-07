namespace EA.Weee.RequestHandlers.Services
{
    using System;
    using Core.DataReturns;
    using DataAccess.DataAccess;

    public class QuarterWindowService : IQuarterWindowService
    {
        private readonly IQuarterWindowTemplateDataAccess dataAccess;

        public QuarterWindowService(IQuarterWindowTemplateDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public QuarterWindow GetQuarterWindow(Quarter quarter)
        {
            var quarterWindowTemplate = dataAccess.GetByQuarter((int)quarter.Q);

            var startDate = new DateTime(quarter.Year + quarterWindowTemplate.AddStartYears, quarterWindowTemplate.StartMonth, quarterWindowTemplate.StartDay);
            var endDate = new DateTime(quarter.Year + quarterWindowTemplate.AddEndYears, quarterWindowTemplate.EndMonth, quarterWindowTemplate.EndDay);

            return new QuarterWindow(quarter, startDate, endDate);
        }
    }
}
