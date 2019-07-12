namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using EA.Weee.Domain.Lookup;

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

            return new QuarterWindow(startDate, endDate, quarter.Q);
        }

        public async Task<QuarterWindow> GetNextQuarterWindow(QuarterType q, int year)
        {
            int next = q == QuarterType.Q4 ? 1 : (int)q + 1;

            QuarterWindowTemplate quarterWindowTemplate = await dataAccess.GetByQuarter(next);

            DateTime startDate = new DateTime(year + quarterWindowTemplate.AddStartYears, quarterWindowTemplate.StartMonth, quarterWindowTemplate.StartDay);
            DateTime endDate = new DateTime(year + quarterWindowTemplate.AddEndYears, quarterWindowTemplate.EndMonth, quarterWindowTemplate.EndDay);

            return new QuarterWindow(startDate, endDate, (QuarterType)next);
        }

        public async Task<QuarterWindow> GetAnnualQuarter(Quarter quarter)
        {
            var quarterWindowTemplate = await dataAccess.GetByQuarter((int)quarter.Q);

            int startMonth;
            if (quarter.Q == QuarterType.Q4)
            {
                startMonth = 10;
            }
            else
            {
                startMonth = quarterWindowTemplate.StartMonth - 3;
            }

            var startDate = new DateTime(quarter.Year, startMonth, quarterWindowTemplate.StartDay);
            var endDateMonth = startDate.Month + quarterWindowTemplate.EndMonth - 1;
            var endDateYear = startDate.Year + quarterWindowTemplate.AddStartYears;
            var endDate = new DateTime(quarter.Year, endDateMonth, DateTime.DaysInMonth(endDateYear, endDateMonth));

            return new QuarterWindow(startDate, endDate, quarter.Q);
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

                    var quarterWindow = new QuarterWindow(startDate, endDate, (QuarterType)item.Quarter);

                    if (quarterWindow.IsInWindow(date))
                    {
                        possibleQuarterWindows.Add(new QuarterWindow(startDate, endDate, (QuarterType)item.Quarter));
                    }
                }
            }
            return possibleQuarterWindows;
        }
    }
}
