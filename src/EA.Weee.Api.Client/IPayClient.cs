namespace EA.Weee.Api.Client
{
    using EA.Weee.Api.Client.Models.Pay;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPayClient : IDisposable
    {
        Task<CreatePaymentResult> CreatePaymentAsync(string idempotencyKey, CreateCardPaymentRequest body);

        Task<PaymentWithAllLinks> GetPaymentAsync(string paymentId);
    }
}
