namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using System;
    using System.Threading.Tasks;

    public class PaymentService : IPaymentService
    {
        private readonly IPayClient paymentClient;
        private readonly ConfigurationService configurationService;
        private readonly ISecureReturnUrlHelper secureReturnUrlHelper;
        private readonly IPaymentReferenceGenerator paymentReferenceGenerator;

        public PaymentService(IPayClient paymentClient, ConfigurationService configuration, ISecureReturnUrlHelper secureReturnUrlHelper, IPaymentReferenceGenerator paymentReferenceGenerator)
        {
            this.paymentClient = paymentClient;
            this.configurationService = configuration;
            this.secureReturnUrlHelper = secureReturnUrlHelper;
            this.paymentReferenceGenerator = paymentReferenceGenerator;
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(int amount, string description)
        {
            var secureId = secureReturnUrlHelper.GenerateSecureRandomString();
            var returnUrl = $"{configurationService.CurrentConfiguration.GovUkPayReturnBaseUrl}/{secureId}";

            var paymentRequest = new CreateCardPaymentRequest
            {
                Amount = amount,
                Description = description,
                Reference = paymentReferenceGenerator.GeneratePaymentReferenceWithSeparators(),
                ReturnUrl = returnUrl
            };

            var idempotencyKey = Guid.NewGuid().ToString();

            var result = await paymentClient.CreatePaymentAsync(idempotencyKey, paymentRequest);

            // Store the data

            return result;
        }

        public async Task HandlePaymentReturnAsync(string secureId)
        {
            if (!secureReturnUrlHelper.ValidateSecureRandomString(secureId))
            {
                throw new InvalidOperationException("Invalid secure ID");
            }

            // check the database

            // Process the payment result...
        }
    }
}