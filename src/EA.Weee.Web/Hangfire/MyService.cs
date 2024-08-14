namespace EA.Weee.Web.Hangfire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Serilog;
    using Serilog.Events;

    public class MyService : IMyService
    {
        private readonly ILogger logger;

        public MyService(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task PerformTask()
        {
            logger.Error("lala");
            logger.Debug("lala");
            logger.Information("lala");
            logger.Information("lala");
            Console.WriteLine("running");
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