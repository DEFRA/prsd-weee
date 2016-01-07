namespace EA.Weee.RequestHandlers.Services
{
    using Core.DataReturns;

    public interface IQuarterWindowService
    {
        QuarterWindow GetQuarterWindow(Quarter quarter);
    }
}
