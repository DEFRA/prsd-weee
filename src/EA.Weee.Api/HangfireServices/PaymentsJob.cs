namespace EA.Weee.Api.HangfireServices
{
    using Hangfire;
    using System;
    using System.Threading.Tasks;

    public class PaymentsJob
    {
        private readonly IPaymentsService paymentsService;

        public PaymentsJob(IPaymentsService paymentsService)
        {
            this.paymentsService = paymentsService;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        public async Task Execute(Guid jobId)
        {
            await paymentsService.RunMopUpJob(jobId);
        }
    }
}