namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges.Errors
{
    using System;
    using System.Collections.Generic;

    public interface IIbisFileDataErrorTranslator
    {
        List<string> MakeFriendlyErrorMessages(List<Exception> errors);
    }
}
