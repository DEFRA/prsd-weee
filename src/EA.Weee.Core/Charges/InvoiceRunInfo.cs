namespace EA.Weee.Core.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
