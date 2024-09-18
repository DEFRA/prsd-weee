namespace EA.Weee.Web.Services
{
    using Azure.Core;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Threading.Tasks;

    public class PaymentService : IPaymentService
    {
        private readonly IPayClient paymentClient;
        private readonly ConfigurationService configurationService;
        private readonly ISecureReturnUrlHelper secureReturnUrlHelper;
        private readonly IPaymentReferenceGenerator paymentReferenceGenerator;
        private readonly Func<IWeeeClient> weeeClient;

        public PaymentService(IPayClient paymentClient, ConfigurationService configuration, ISecureReturnUrlHelper secureReturnUrlHelper, IPaymentReferenceGenerator paymentReferenceGenerator, Func<IWeeeClient> weeeClient)
        {
            this.paymentClient = paymentClient;
            this.configurationService = configuration;
            this.secureReturnUrlHelper = secureReturnUrlHelper;
            this.paymentReferenceGenerator = paymentReferenceGenerator;
            this.weeeClient = weeeClient;
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(Guid directRegistrantId, string email, string accessToken)
        {
            var secureId = secureReturnUrlHelper.GenerateSecureRandomString();
            var returnUrl = $"{configurationService.CurrentConfiguration.GovUkPayReturnBaseUrl}/{secureId}";

            var paymentRequest = new CreateCardPaymentRequest
            {
                Amount = configurationService.CurrentConfiguration.GovUkPaymentAmountPence,
                Description = configurationService.CurrentConfiguration.GovUkPayDescription,
                Reference = paymentReferenceGenerator.GeneratePaymentReferenceWithSeparators(),
                ReturnUrl = returnUrl,
                Email = email 
            };

            var idempotencyKey = Guid.NewGuid().ToString();

            var result = await paymentClient.CreatePaymentAsync(idempotencyKey, paymentRequest);

            using (var client = weeeClient())
            {
                await client.SendAsync(accessToken, new UpdateSubmissionPaymentDetailsRequest(directRegistrantId, paymentRequest.Reference, secureId, result.PaymentId));
            }

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