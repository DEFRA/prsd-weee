namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Core.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreatePaymentAsync(Guid directRegistrantId, string email, string accessToken);

        Task<PaymentResult> HandlePaymentReturnAsync(string accessToken, string token);

        bool ValidateExternalUrl(string url);
    }
}