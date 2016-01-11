namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IQuarterFactory
    { 
        Task SetFixedQuarter(Quarter quarter);

        Task ToggleFixedQuarterUseage(bool enabled);
    }
}
