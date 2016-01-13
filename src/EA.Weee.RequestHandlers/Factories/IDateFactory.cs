namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;

    public interface IDateFactory
    { 
        Task SetFixedDate(DateTime quarter);

        Task ToggleFixedDateUsage(bool enabled);
    }
}
