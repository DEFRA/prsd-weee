namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Requests.AatfEvidence;
    using Requests.Scheme;

    public interface ITransferTonnagesValidator
    {
        Task Validate(List<TransferTonnageValue> transferValues);
    }
}