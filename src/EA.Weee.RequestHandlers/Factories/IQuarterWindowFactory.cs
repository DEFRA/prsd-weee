namespace EA.Weee.RequestHandlers.Factories
{
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IQuarterWindowFactory
    {
        Task<QuarterWindow> GetQuarterWindow(Quarter quarter);
    }
}
