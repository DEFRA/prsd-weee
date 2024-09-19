namespace EA.Weee.Web.Services
{
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.DirectRegistrant;

    public class PaymentService : IPaymentService
    {
        private readonly string[] allowedDomains = new[] { "publicapi.payments.service.gov.uk", "card.payments.service.gov.uk" };

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
            var returnUrl = string.Format(configurationService.CurrentConfiguration.GovUkPayReturnBaseUrl, directRegistrantId.ToString(), secureId);

            var paymentRequest = new CreateCardPaymentRequest
            {
                Amount = configurationService.CurrentConfiguration.GovUkPayAmountInPence,
                Description = configurationService.CurrentConfiguration.GovUkPayDescription,
                Reference = paymentReferenceGenerator.GeneratePaymentReferenceWithSeparators(),
                ReturnUrl = returnUrl,
                Email = email 
            };

            var idempotencyKey = Guid.NewGuid().ToString();

            var result = await paymentClient.CreatePaymentAsync(idempotencyKey, paymentRequest);

            using (var client = weeeClient())
            {
                await client.SendAsync(accessToken, new AddPaymentSessionRequest(directRegistrantId, paymentRequest.Reference, secureId, result.PaymentId, paymentRequest.Amount));
            }

            return result;
        }

        public async Task<PaymentResult> HandlePaymentReturnAsync(string accessToken, string token)
        {
            if (!secureReturnUrlHelper.ValidateSecureRandomString(token))
            {
                throw new InvalidOperationException("Invalid secure ID");
            }

            using (var client = weeeClient())
            {
                var payment = await client.SendAsync(accessToken, new ValidateAndGetSubmissionPayment(token));

                if (!string.IsNullOrWhiteSpace(payment.ErrorMessage))
                {
                    throw new InvalidOperationException($"ProcessPaymentResultAsync invalid token {payment.ErrorMessage}");
                }

                var result = await paymentClient.GetPaymentAsync(payment.PaymentId);

                await client.SendAsync(accessToken, new UpdateSubmissionPaymentDetailsRequest(payment.DirectRegistrantId, result.State.Status, payment.PaymentSessionId));

                if (result.State.Status == PaymentStatus.Success)
                {
                    return new PaymentResult()
                    {
                        PaymentReference = payment.PaymentReference,
                        DirectRegistrantId = payment.DirectRegistrantId
                    };
                }

                return new PaymentResult()
                {
                    DirectRegistrantId = payment.DirectRegistrantId
                };
            }
        }

        public bool ValidateExternalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            try
            {
                var uri = new Uri(url);
                return allowedDomains.Any(domain => uri.Host.Equals(domain, StringComparison.OrdinalIgnoreCase));
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
    }
}