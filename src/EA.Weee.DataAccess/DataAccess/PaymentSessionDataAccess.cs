namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class PaymentSessionDataAccess : IPaymentSessionDataAccess
    {
        private readonly WeeeContext weeeContext;
        private readonly IUserContext userContext;
        
        public PaymentSessionDataAccess(WeeeContext weeeContext, IUserContext userContext)
        {
            this.weeeContext = weeeContext;
            this.userContext = userContext;
        }

        public async Task<PaymentSession> GetCurrentInProgressPayment(string paymentToken, Guid directRegistrantId, int year)
        {
            var validInProgressStatus = new List<int>() { PaymentState.Capturable.Value, PaymentState.Started.Value, PaymentState.Created.Value, PaymentState.New.Value };

            return await weeeContext.PaymentSessions.Where(c =>
                    c.PaymentReturnToken == paymentToken &&
                    c.UserId.ToString() == userContext.UserId.ToString() &&
                    validInProgressStatus.Contains(c.Status.Value) &&
                    c.DirectRegistrantId == directRegistrantId &&
                    c.DirectProducerSubmission.ComplianceYear == year &&
                    c.InFinalState == false).OrderByDescending(p => p.CreatedAt)
                .Include(paymentSession => paymentSession.DirectRegistrant).FirstOrDefaultAsync();
        }

        public async Task<PaymentSession> GetCurrentRetryPayment(Guid directRegistrantId, int year)
        {
            return await weeeContext.PaymentSessions.Where(c =>
                    c.UserId.ToString() == userContext.UserId.ToString() &&
                    c.DirectRegistrantId == directRegistrantId &&
                    (c.Status.Value == PaymentState.Created.Value || c.Status.Value == PaymentState.Started.Value 
                    || c.Status.Value == PaymentState.Submitted.Value || c.Status.Value == PaymentState.New.Value) &&
                    c.DirectProducerSubmission.ComplianceYear == year &&
                    c.InFinalState == false).OrderByDescending(p => p.CreatedAt)
                .Include(paymentSession => paymentSession.DirectRegistrant).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyPaymentTokenAsync(string paymentToken)
        {
            return await weeeContext.PaymentSessions.AnyAsync(c => c.PaymentReturnToken == paymentToken);
        }

        public async Task<PaymentSession> GetByIdAsync(Guid paymentSessionId)
        {
            return await weeeContext.PaymentSessions.FirstOrDefaultAsync(p => p.Id == paymentSessionId);
        }

        public async Task<List<PaymentSession>> GetIncompletePaymentSessions(int windowMinutes, int lastProcessMinutes)
        {
            var threeHoursAgo = SystemTime.UtcNow.AddMinutes(windowMinutes);
            var lastProcessed = SystemTime.UtcNow.AddMinutes(lastProcessMinutes);

            return await weeeContext.PaymentSessions
                .Where(p => !p.InFinalState && (p.CreatedAt < threeHoursAgo) &&
                            (p.LastProcessedAt == null || p.LastProcessedAt < lastProcessed))
                .ToListAsync();
        }
    }
}
