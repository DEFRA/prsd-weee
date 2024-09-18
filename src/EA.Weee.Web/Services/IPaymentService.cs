namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client.Models.Pay;
    using System.Threading.Tasks;

    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreatePaymentAsync(int amount, string description);
        Task HandlePaymentReturnAsync(string secureId);
    }
}