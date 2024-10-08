namespace EA.Weee.Api.HangfireServices
{
    using System.Threading.Tasks;

    public interface IPaymentsService
    {
        Task PerformTask();
    }
}
