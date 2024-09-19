namespace EA.Weee.Core.DirectRegistrant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SubmissionPaymentDetails
    {
        public string ErrorMessage { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public string PaymentId { get; set; }

        public string PaymentReference { get; set; }

        public Guid PaymentSessionId { get; set; }
    }
}
