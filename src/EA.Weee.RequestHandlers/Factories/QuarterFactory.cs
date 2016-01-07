namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.DataReturns;

    public class QuarterFactory : IQuarterFactory
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly WeeeContext context;

        public QuarterFactory(ISystemDataDataAccess systemDataDataAccess, WeeeContext context)
        {
            this.systemDataDataAccess = systemDataDataAccess;
            this.context = context;
        }

        public async Task<Quarter> GetCurrent()
        {
            var systemData = await systemDataDataAccess.Get();

            if (systemData.UseFixedComplianceYearAndQuarter)
            {
                return new Quarter(systemData.FixedComplianceYear, (QuarterType)(int)systemData.FixedQuarter);
            }

            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    return new Quarter(DateTime.Now.Year, QuarterType.Q1);
                case 4:
                case 5:
                case 6:
                    return new Quarter(DateTime.Now.Year, QuarterType.Q2);
                case 7:
                case 8:
                case 9:
                    return new Quarter(DateTime.Now.Year, QuarterType.Q3);
                case 10:
                case 11:
                case 12:
                    return new Quarter(DateTime.Now.Year, QuarterType.Q4);
                default:
                    throw new IndexOutOfRangeException("The current quarter month is not in the range 1-12");
            }
        }

        public async Task SetCurrent(Quarter quarter)
        {
            var systemData = await systemDataDataAccess.Get();

            systemData.UpdateQuarterAndComplianceYear(quarter);

            await context.SaveChangesAsync();
        }
    }
}
