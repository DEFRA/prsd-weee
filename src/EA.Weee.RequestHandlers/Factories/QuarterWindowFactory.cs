namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
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

        public async Task<List<QuarterWindow>> GetQuarterWindowsForDate(DateTime date)
        {
            var possibleComplianceYears = new int[] { date.Year - 1, date.Year, date.Year + 1 };
            var possibleQuarterWindows = new List<QuarterWindow>();

            var allQuarterWindowTemplates = await dataAccess.GetAll();

            foreach (var possibleComplianceYear in possibleComplianceYears)
            {
                foreach (var item in allQuarterWindowTemplates)
                {
                    var startDate = new DateTime(possibleComplianceYear + item.AddStartYears, item.StartMonth, item.StartDay);
                    var endDate = new DateTime(possibleComplianceYear + item.AddEndYears, item.EndMonth, item.EndDay);

                    var quarterWindow = new QuarterWindow(startDate, endDate);

                    if (quarterWindow.IsInWindow(date))
                    {
                        possibleQuarterWindows.Add(new QuarterWindow(startDate, endDate));
                    }
                }
            }
            return possibleQuarterWindows;
        }
    }
}
