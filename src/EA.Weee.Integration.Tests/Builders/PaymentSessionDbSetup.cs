namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System;

    public class PaymentSessionDbSetup : DbTestDataBuilder<PaymentSession, PaymentSessionDbSetup>
    {
        protected override PaymentSession Instantiate()
        {
            instance = new PaymentSession()
            {
                PaymentId = "PaymentId02",
                PaymentReference = "PaymentReference03",
                PaymentReturnToken = "http://return",
                Amount = 10m,
            };

            return instance;
        }

        public PaymentSessionDbSetup WithUser(string userId)
        {
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.UserId, userId, instance);

            return this;
        }

        public PaymentSessionDbSetup WithStatus(PaymentState state)
        {
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.Status, state, instance);

            return this;
        }

        public PaymentSessionDbSetup WithPaymentTokenUrl(string tokenUrl)
        {
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.PaymentReturnToken, tokenUrl, instance);

            return this;
        }

        public PaymentSessionDbSetup WithReference(string reference)
        {
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.PaymentReference, reference, instance);

            return this;
        }

        public PaymentSessionDbSetup WithDirectRegistrantSubmission(DirectProducerSubmission submission)
        {
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.DirectProducerSubmissionId, submission.Id, instance);
            ObjectInstantiator<PaymentSession>.SetProperty(o => o.DirectRegistrantId, submission.DirectRegistrantId, instance);

            return this;
        }
    }
}
