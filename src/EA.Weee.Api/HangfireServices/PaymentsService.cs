namespace EA.Weee.Api.HangfireServices
{
    using Serilog;
    using System.Threading.Tasks;

    public class PaymentsService : IPaymentsService
    {
        private readonly ILogger logger;

        public PaymentsService(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task PerformTask()
        {
        }
        //public async Task RunMopUpJob()
        //{
        //    var threeHoursAgo = DateTime.UtcNow.AddHours(-3);
        //    var incompletePayments = await _context.Payments
        //        .Where(p => p.Status == "pending" && p.CreatedAt < threeHoursAgo && !p.Processed
        //               && (p.LastProcessedAt == null || p.LastProcessedAt < DateTime.UtcNow.AddMinutes(-30)))
        //        .ToListAsync();

        //    foreach (var payment in incompletePayments)
        //    {
        //        try
        //        {
        //            await ProcessPayment(payment);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, $"Error processing payment {payment.PaymentId}");
        //        }
        //    }
        //}

        //private async Task ProcessPayment(Payment payment)
        //{
        //    // Use a transaction to ensure atomicity
        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        // Double-check the payment hasn't been processed
        //        var freshPayment = await _context.Payments.FindAsync(payment.Id);
        //        if (freshPayment.Processed)
        //        {
        //            _logger.LogInformation($"Payment {payment.PaymentId} already processed. Skipping.");
        //            return;
        //        }

        //        var status = await _paymentService.GetPaymentStatus(payment.PaymentId);
        //        freshPayment.Status = status;
        //        freshPayment.Processed = true;
        //        freshPayment.LastProcessedAt = DateTime.UtcNow;

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}
    }
}