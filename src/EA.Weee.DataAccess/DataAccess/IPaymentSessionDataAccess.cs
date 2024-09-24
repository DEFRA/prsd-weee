namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;

    public interface IPaymentSessionDataAccess
    {
        Task<PaymentSession> GetCurrentInProgressPayment(string paymentToken, Guid directRegistrantId, int year);

        Task<PaymentSession> GetCurrentRetryPayment(Guid directRegistrantId, int year);
    }
}