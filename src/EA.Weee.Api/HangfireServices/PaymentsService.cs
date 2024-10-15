namespace EA.Weee.Api.HangfireServices
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using Serilog;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class PaymentsService : IPaymentsService
    {
        private readonly ILogger logger;
        private readonly WeeeContext context;
        private readonly IWeeeTransactionAdapter transactionAdapter;
        private readonly IPayClient payClient;
        private readonly IPaymentSessionDataAccess paymentSessionDataAccess;

        public PaymentsService(ILogger logger, WeeeContext context, IWeeeTransactionAdapter transactionAdapter, IPayClient payClient, IPaymentSessionDataAccess paymentSessionDataAccess)
        {
            this.logger = logger;
            this.context = context;
            this.transactionAdapter = transactionAdapter;
            this.payClient = payClient;
            this.paymentSessionDataAccess = paymentSessionDataAccess;
        }

        public async Task RunMopUpJob(Guid jobId)
        {
            logger.Information($"Starting RunMopUpJob. Job ID: {jobId}");

            try
            {
                var incompletePayments = await paymentSessionDataAccess.GetIncompletePaymentSessions();

                logger.Information($"Found {incompletePayments.Count} incomplete payments to process. Job ID: {jobId}");

                foreach (var payment in incompletePayments)
                {
                    try
                    {
                        await ProcessPayment(payment, jobId);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Error processing payment {payment.PaymentId}. Job ID: {jobId}");
                    }
                }

                logger.Information($"Completed RunMopUpJob. Job ID: {jobId}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error in RunMopUpJob. Job ID: {jobId}");
            }
        }

        private async Task ProcessPayment(PaymentSession payment, Guid jobId)
        {
            using (var transaction = transactionAdapter.BeginTransaction())
            {
                try
                {
                    // Double-check the payment hasn't been processed
                    var freshPayment = await paymentSessionDataAccess.GetByIdAsync(payment.Id);

                    if (freshPayment != null)
                    {
                        if (freshPayment.InFinalState)
                        {
                            logger.Information($"Payment {payment.PaymentId} already processed. Skipping. Job ID: {jobId}");
                            return;
                        }
                        var status = await payClient.GetPaymentAsync(payment.PaymentId);

                        if (status != null)
                        {
                            freshPayment.Status = status.State.Status.ToDomainEnumeration<PaymentState>();
                            freshPayment.InFinalState = status.State.IsInFinalState();
                        }
                        else 
                        {
                            freshPayment.Status = PaymentState.Error;
                            freshPayment.InFinalState = true;

                            logger.Information($"Payment {payment.PaymentId} not found in GOV.UK pay. Setting it to error. Job ID: {jobId}");
                        }

                        freshPayment.DirectProducerSubmission.FinalPaymentSession = freshPayment;
                        freshPayment.LastProcessedAt = DateTime.UtcNow;
                        
                        context.SetCurrentJobId(jobId);

                        await context.SaveChangesAsync(CancellationToken.None);
                        transaction.Commit();

                        logger.Information($"Successfully processed payment {payment.PaymentId}. Job ID: {jobId}");
                    }
                    else
                    {
                        logger.Information($"Payment {payment.PaymentId} not found. Skipping. Job ID: {jobId}");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Error processing payment {payment.PaymentId}. Job ID: {jobId}");
                    transaction.Commit();
                    throw;
                }
            }
        }
    }
}