namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPaymentSessionDataAccess
    {
        Task<PaymentSession> GetCurrentInProgressPayment(string paymentToken, Guid directRegistrantId, int year);

        Task<PaymentSession> GetCurrentPayment(string paymentToken, Guid directRegistrantId, int year);

        Task<PaymentSession> GetCurrentRetryPayment(Guid directRegistrantId, int year);

        Task<bool> AnyPaymentTokenAsync(string paymentToken);

        Task<PaymentSession> GetByIdAsync(Guid paymentSessionId);

        Task<List<PaymentSession>> GetIncompletePaymentSessions(int windowMinutes, int lastProcessMinutes);
    }
}