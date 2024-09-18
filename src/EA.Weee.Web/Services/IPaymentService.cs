namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client.Models.Pay;
    using System;
    using System.Threading.Tasks;

    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreatePaymentAsync(Guid directRegistrantId, string email, string accessToken);

        Task<bool> HandlePaymentReturnAsync(string accessToken, Guid directRegistrantId, string token);

        bool ValidateExternalUrl(string url);
    }
}