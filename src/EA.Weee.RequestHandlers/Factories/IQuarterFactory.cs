namespace EA.Weee.RequestHandlers.Factories
{
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IQuarterFactory
    {
        Task<Quarter> GetCurrent();

        Task SetFixedQuarter(Quarter quarter);

        Task ToggleFixedQuarterUseage(bool enabled);
    }
}
