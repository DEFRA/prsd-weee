namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IQuarterWindowFactory
    {
        Task<QuarterWindow> GetQuarterWindow(Quarter quarter);

        Task<List<QuarterWindow>> GetQuarterWindowsForDate(DateTime date);

        Task<QuarterWindow> GetAnnualQuarter(Quarter quarter);
    }
}
