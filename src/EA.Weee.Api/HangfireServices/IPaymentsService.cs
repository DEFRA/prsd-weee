namespace EA.Weee.Api.HangfireServices
{
    using System;
    using System.Threading.Tasks;

    public interface IPaymentsService
    {
        Task RunMopUpJob(Guid jobId);
    }
}
