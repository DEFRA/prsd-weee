namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates 1B1S transaction references for the WEEE system which are
    /// guarenteed to be unique.
    /// </summary>
    public interface ITransactionReferenceGenerator
    {
        Task<string> GetNextTransactionReferenceAsync();
    }
}
