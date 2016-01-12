namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IDateFactory
    { 
        Task SetFixedDate(DateTime quarter);

        Task ToggleFixedDateUsage(bool enabled);
    }
}
