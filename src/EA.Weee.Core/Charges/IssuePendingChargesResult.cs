namespace EA.Weee.Core.Charges
{
    using System;
    using System.Collections.Generic;

    public class IssuePendingChargesResult
    {
        public Guid? InvoiceRunId { get; set; }

        public List<string> Errors { get; set; }
    }
}
