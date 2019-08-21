namespace EA.Weee.Core.Charges
{
    using System;

    /// <summary>
    /// Provides details about an invoice run.
    /// </summary>
    public class InvoiceRunInfo
    {
        public Guid InvoiceRunId { get; set; }

        public DateTime IssuedDate { get; set; }

        public string IssuedByUserFullName { get; set; }
    }
}
